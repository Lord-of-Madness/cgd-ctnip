using System;
using System.ComponentModel;
using Unity.AI.Navigation.Samples;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("CharacterName")]
	public string charName = "Beth";
	[Header("Jumping")]
	[SerializeField]
	float jumpForce = 10f;
	[SerializeField]
	float minTimeBetweenJumps = 0.5f;
	[SerializeField]
	float jumpTime = 2;

	bool isGrounded = true;
	float timeSinceJump = 0;

	[Header("Movement")]
	[SerializeField]
	float speed = 100;
	[SerializeField]
	bool isoMovement = true;
	[SerializeField]
	bool controlledByPlayer = true;

	Vector3 curVelocity = Vector3.zero;

	[Header("References")]
	[SerializeField]
	OverheadDialogue overheadDialogue;
	[SerializeField]
	GameObject bodyArmature;

	//Animation stuff
	public Animator bodyAnimator;
	string animSpeedID = "Speed";
	string animJumpID = "Jump";
	string animGroundedID = "Grounded";
	string animFreeFallID = "FreeFall";
	string animMotionSpeedID = "MotionSpeed";

	private InputActionsGen inputActions;
	public PlayerData playerData;
	public UnityEvent onToolUsed;
	/*
	public class ReloadEventData// might be useful later.
	{
		public int count;
		public ReloadEventData(int count)
		{
			this.count = count;
		}
	}*/
	public UnityEvent/*<ReloadEventData>*/ onReload;

	private void Awake()
	{
		inputActions = new();
		inputActions.Player.Enable();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (bodyArmature != null) { 
			bodyAnimator = bodyArmature.GetComponent<Animator>();
		}
		else { Debug.LogWarning("Armature not set! Can't save the animator component"); }


		playerData = GetComponent<PlayerData>();
		inputActions.Player.Jump.performed += (ctx) => { if (isGrounded && controlledByPlayer) Jump();};
		inputActions.Player.Attack.performed += (ctx) => { if (controlledByPlayer) Attack(ctx); };
		inputActions.Player.Reload.performed += (ctx) => { if (controlledByPlayer) Reload(ctx); };

		Physics.gravity = new Vector3(0, -20, 0);
	}

	private void Attack(InputAction.CallbackContext context)
	{
		if (playerData.TryFire())//The numerical changes are done in the PlayerData class
		{
			onToolUsed.Invoke();
			//TODO firing rays/projectiles, flashing flashes, animations and stuff
		}
		else
		{
			//TODO: Play "Out of ammo!"
		}
	}
	private void Reload(InputAction.CallbackContext context)
	{
		if (playerData.TryReload())//This is true only if there is a reason to actually reload - there is ammo to reload and the tool is not full
		{//The numerical changes are done in the PlayerData class
			onReload.Invoke();
			//TODO animaèky and stuff
		}
	}

	// Update is called once per frame
	void Update()
	{
		// 4. Read the "Move" action value, which is a 2D vector
		// and the "Jump" action state, which is a boolean value

		Vector2 moveValue = inputActions.Player.Move.ReadValue<Vector2>();
		MoveByKeyboard(moveValue);
		timeSinceJump += Time.deltaTime;

		if (controlledByPlayer)
		{
			Move();
		}
		RotateInMoveDir();

		if (curVelocity.y > 0) curVelocity.y -= (Time.deltaTime / jumpTime) * jumpForce;
		else curVelocity.y = 0;

		//Animator set values
		if (controlledByPlayer)
		{
			bodyAnimator.applyRootMotion = true;

			bodyAnimator.SetBool(animGroundedID, isGrounded);
			bodyAnimator.SetBool(animJumpID, curVelocity.y > 0);

			if (curVelocity.magnitude > 0) bodyAnimator.SetFloat(animMotionSpeedID, 1);
			else bodyAnimator.SetFloat(animMotionSpeedID, 1);

			bodyAnimator.SetFloat(animSpeedID, curVelocity.magnitude * 10);
		}

		else
		{
			bodyAnimator.applyRootMotion = false;
		}
	}

	void Move()
	{
		//Debug.Log("Moving in dir: " + curVelocity.ToString());
		transform.position += curVelocity * Time.deltaTime;

	}

	void RotateInMoveDir()
	{
		if (!controlledByPlayer) { bodyArmature.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); return; }
		//Rotate in direction of velocity
		if (curVelocity != Vector3.zero)
		{
			Quaternion bodyRot = Quaternion.FromToRotation(Vector3.forward, new Vector3(curVelocity.x, 0, curVelocity.z));

			//For some reason in this case the rotation was (-180,0,0) which made the character disappear -> no better fix found
			if (Vector3.forward.normalized == -new Vector3(curVelocity.x, 0, curVelocity.z).normalized)
				bodyRot = Quaternion.Euler(0, -180, 0);

			//Debug.Log("Rotating body to " + bodyRot.ToString() + " and curVelocity is: " + curVelocity);
			bodyArmature.transform.rotation = bodyRot.normalized;
		}
	}

	void MoveByKeyboard(Vector2 dir)
	{
		Vector3 moveVelocity = new Vector3(dir.x, 0, dir.y) * speed;

		if (isoMovement) moveVelocity = Quaternion.Euler(0, 45, 0) * moveVelocity;

		curVelocity.x = moveVelocity.x;
		curVelocity.z = moveVelocity.z;
	}

	void Jump()
	{
		timeSinceJump = 0;
		//Debug.Log("Fire jump");
		curVelocity.y = jumpForce;
		isGrounded = false;

		bodyArmature.GetComponent<Animator>().SetBool("Jump", true);

	}


	//Called from FeetCollider
	public void FeetTriggerStay()
	{
		//Debug.Log("Feet collision detected in " + charName);
		//This is because collider can collider the next frame after jump and instantly enable jumping again
		if (timeSinceJump > minTimeBetweenJumps)
			isGrounded = true;
	}

	//Called from FeetCollider
	public void FeetTriggerExit()
	{
		//Debug.Log("Feet collision exit detected");
		//This is because collider can collider the next frame after jump and instantly enable jumping again
		isGrounded = false;
	}


	private void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Interactable"))
			overheadDialogue.ShowText(collision.gameObject.GetComponent<InteractableScript>().commentLines);

	}

	public void StopFollowingOtherChar()
	{
		gameObject.GetComponent<NavMeshAgent>().enabled = false;
		gameObject.GetComponent<AITarget>().enabled = false;
		gameObject.GetComponent<AgentLinkMover>().enabled = false;
	}

	public void StartFollowingOtherChar()
	{
		gameObject.GetComponent<NavMeshAgent>().enabled = true;
		gameObject.GetComponent<AITarget>().enabled = true;
		gameObject.GetComponent<AgentLinkMover>().enabled = true;
	}

	public bool IsControlledByPlayer() => controlledByPlayer;

	public void DisablePlayerControl()
	{
		controlledByPlayer = false;
	}

	public void EnablePlayerControl()
	{
		controlledByPlayer = true;
	}
}
