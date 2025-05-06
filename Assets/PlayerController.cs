using System;
using System.ComponentModel;
using Unity.AI.Navigation.Samples;
using Unity.Burst;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Search;
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
    [SerializeField]
    bool isRunning = false;

    Vector3 curVelocity = Vector3.zero;

    [Header("Combat")]
    [Header("Beth")]
    [SerializeField]
    [Tooltip("This is an offset of the gun when held in hand. Set only if the character holds a gun. X = horizontal, Y = vertical")]
    Vector2 weaponOffset = new Vector2(0.2f, 1f);
    [SerializeField]
    int gunDamage = 10;
    [SerializeField]
	[Tooltip("Reference to a bullet prefab. Set only if the character can shoot with a gun")]
	BulletScript bulletPrefab;

    bool hasLineRenderer = false;
	bool aimLaserVisible = false;
	Vector3 curAimDir = Vector3.zero;

    [Header("Erik")]
    [SerializeField]
    [Tooltip("Reference to the melee attack hit box script. Set only for characters with melee weapons")]
    AttackHitScript meleeAttackHitScript;
    [SerializeField]
    float meleeAttackTime = 1.5f;
    [SerializeField]
    float meleeAttackForce = 10;


	float timeAttacking = 0;
    bool meleeAttacking = false;
    bool dealtMeleeDamage = false;


	[Header("References")]
    [SerializeField]
    OverheadDialogue overheadDialogue;
    [SerializeField]
    GameObject bodyArmature;
    [SerializeField]
    [Tooltip("This is a reference to a lineRenderer which draws a laser aim. Set only for characters which aim this way.")]
    LineRenderer lineRenderer;


    //Animation stuff
    Animator bodyAnimator;


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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bodyArmature != null)
        {
            bodyAnimator = bodyArmature.GetComponent<Animator>();
        }
        else { Debug.LogWarning("Armature not set! Can't save the animator component"); }


        playerData = GetComponent<PlayerData>();
        GameManager.Instance.inputActions.Player.Jump.performed += (ctx) => { if (isGrounded && controlledByPlayer) Jump(); };
        GameManager.Instance.inputActions.Player.Sprint.performed += (ctx) => { if (controlledByPlayer) ToggleRunning(); };
		GameManager.Instance.inputActions.Player.Aim.started += (ctx) => { if (controlledByPlayer) ShowLaserAim(); };
		GameManager.Instance.inputActions.Player.Aim.canceled += (ctx) => { if (controlledByPlayer) HideLaserAim(); };
		GameManager.Instance.inputActions.Player.Attack.performed += (ctx) => { if (controlledByPlayer) Attack(ctx); };
		GameManager.Instance.inputActions.Player.Reload.performed += (ctx) => { if (controlledByPlayer) Reload(ctx); };

        Physics.gravity = new Vector3(0, -20, 0);

        if (controlledByPlayer) StopFollowingOtherChar();
        if (lineRenderer != null) hasLineRenderer = true;
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (playerData.CanUseTool())
        {
            if (playerData.SelectedTool.toolName == GlobalConstants.revolverToolName)
            {
                if (aimLaserVisible)//The numerical changes are done in the PlayerData class
                {
                    if (playerData.TryFire())
                        ShootFromGun();
                }

            }
            //Something else than gun with which you must aim
            else
            {
                if (playerData.TryFire())
                {
                    if (playerData.SelectedTool.toolName == GlobalConstants.pipeToolName)
                    {
                        MeleeAttack();
                    }
                }
            }

        }
        //OUT OF AMMO
        else
        {
            Debug.Log("Out of ammo");
            //TODO: Play "Out of ammo!"
        }
    }

    void MeleeAttack()
    {
		//Stop following target
		meleeAttacking = true;

        //Attack animation
		bodyAnimator.SetBool(GlobalConstants.animAttackID, true);
	}

	void MeleeAttackTimingManagment()
    {
		if (meleeAttacking)
			timeAttacking += Time.deltaTime;

		if (timeAttacking > meleeAttackTime / 3 && !dealtMeleeDamage)
			DealMeleeDamage();

		if (timeAttacking > meleeAttackTime)
			FinishMeleeAttack();

	}

	void DealMeleeDamage()
    {
		foreach (Collider c in meleeAttackHitScript.GetAllObjectsInAttackArea())
		{
            if (c.CompareTag("Enemy"))
            {
                //Debug.Log("Applying force to an enemy!");
                //Apply force to object rigidbody (no real damage done)
                if (c.transform.parent != null)
                {
                    EnemyScript enemy = c.transform.parent.GetComponent<EnemyScript>();
                    Vector3 appliedForce = (enemy.transform.position - transform.position).normalized * meleeAttackForce*50000;
					                                                            //50000 is the magic constant which make the enemy rigibody fly a bit

					enemy.GetComponent<Rigidbody>().AddForce(appliedForce);
                    enemy.GetHit(0);
                }
            }
		}

		dealtMeleeDamage = true;

    }

    void FinishMeleeAttack()
    {
        timeAttacking = 0;
        dealtMeleeDamage = false;
        meleeAttacking = false;
        bodyAnimator.SetBool(GlobalConstants.animAttackID, false);
    }

    void ShootFromGun()
    {
        RaycastHit hit;
        Vector3 gunPos = transform.position +
            new Vector3(bodyArmature.transform.forward.x * weaponOffset.x,
            weaponOffset.y,
            bodyArmature.transform.forward.z * weaponOffset.x);


        Ray ray = new Ray(gunPos, curAimDir);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer("UI")))
		{
			SpawnBullet(gunPos, curAimDir, 100, (transform.position - hit.point).magnitude / 100);
			if (hit.collider.CompareTag("Enemy"))
			{
				EnemyScript enemy = hit.collider.transform.parent.GetComponent<EnemyScript>();
				if (enemy != null) enemy.GetHit(gunDamage);
				else Debug.Log("Collider tagged 'Enemy' didn't find EnemyScript in parent");
			}
		}
		//MISS
		else
		{
			SpawnBullet(gunPos, curAimDir, 100f, 10f);
		}



		onToolUsed.Invoke();

		//Try to camera shake
		CameraEffectsScript camEff = Camera.main.GetComponent<CameraEffectsScript>();
		if (camEff != null)
			camEff.CameraShake();

		//TODO animations and stuff
	}

	void SpawnBullet(Vector3 spawnPos, Vector3 dir, float speed, float duration)
    {
		BulletScript bullet = Instantiate(bulletPrefab);
		bullet.Direction = dir;
		bullet.Speed = speed;
		bullet.Duration = duration;
		bullet.transform.position = spawnPos;
	}

    private void Reload(InputAction.CallbackContext context)
    {
        //TODO only shoot if aiming
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

        Vector2 moveValue = GameManager.Instance.inputActions.Player.Move.ReadValue<Vector2>();
        MoveByKeyboard(moveValue);

        timeSinceJump += Time.deltaTime;

        if (controlledByPlayer)
            MeleeAttackTimingManagment();


        if (controlledByPlayer && !aimLaserVisible && !meleeAttacking)
        {
            Move();
            RotateInMoveDir();

            SetAnimatorValuesMovement();



        }
        else if (!controlledByPlayer)
        {
            bodyAnimator.applyRootMotion = false;
        }

        if (curVelocity.y > 0) curVelocity.y -= (Time.deltaTime / jumpTime) * jumpForce;
        else curVelocity.y = 0;


        if (hasLineRenderer && aimLaserVisible && controlledByPlayer) { 
            DrawLaserAim();

        }
    }

    void SetAnimatorValuesMovement()
    {

		bodyAnimator.applyRootMotion = true;

		bodyAnimator.SetBool(GlobalConstants.animGroundedID, isGrounded);
		bodyAnimator.SetBool(GlobalConstants.animJumpID, curVelocity.y > 0);

		if (curVelocity.magnitude > 0) bodyAnimator.SetFloat(GlobalConstants.animMotionSpeedID, 1);
		else bodyAnimator.SetFloat(GlobalConstants.animMotionSpeedID, 1);

		bodyAnimator.SetFloat(GlobalConstants.animSpeedID, curVelocity.magnitude * 1);
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
        if (isRunning) moveVelocity *= 2;

        if (isoMovement) moveVelocity = Quaternion.Euler(0, 45, 0) * moveVelocity;

        curVelocity.x = moveVelocity.x;
        curVelocity.z = moveVelocity.z;
    }

    void ToggleRunning()
    {
        isRunning = !isRunning;
    }

    void Jump()
    {
        timeSinceJump = 0;
        //Debug.Log("Fire jump");
        curVelocity.y = jumpForce;
        isGrounded = false;

        bodyAnimator.SetBool(GlobalConstants.animJumpID, true);

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
        //gameObject.GetComponent<NavMeshAgent>().enabled = false;
        gameObject.GetComponent<AITarget>().SetFollowing(false);
        //gameObject.GetComponent<AgentLinkMover>().enabled = false;
    }

    public void StartFollowingOtherChar()
    {
        //gameObject.GetComponent<NavMeshAgent>().enabled = true;
        gameObject.GetComponent<AITarget>().SetFollowing(true);
        //gameObject.GetComponent<AgentLinkMover>().enabled = true;
    }

    public bool IsControlledByPlayer() => controlledByPlayer;

    public void DisablePlayerControl()
    {
        controlledByPlayer = false;
        HideLaserAim();
        //Debug.Log("Disabling char: " + name);
        transform.rotation = Quaternion.identity;
        bodyArmature.transform.rotation = Quaternion.identity;
    }

    public void EnablePlayerControl()
    {
        controlledByPlayer = true;
    }

    /// <summary>
    /// Don't call this if object doesn't have a lineRenderer assigned. Sets curAimDir to a value
    /// </summary>
    void DrawLaserAim(out Vector3 laserDir)
    {
        Debug.Log("Drawing laser aim");

        //Get the position of the gun
        Vector3 startPos = transform.position;
        startPos += new Vector3(bodyArmature.transform.forward.x * weaponOffset.x, weaponOffset.y, bodyArmature.transform.forward.z * weaponOffset.x);

        //Get the position of mouse direciton intersecion with the plane of the gun
        Vector3 mouseDir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        Vector3 mouseLaserToGunPlanePoint;
        if (!Utilities.LinePlaneIntersection(out mouseLaserToGunPlanePoint, Camera.main.transform.position, mouseDir, Vector3.up, startPos))
            mouseLaserToGunPlanePoint = startPos + Vector3.forward;

        //Draw the line via saved lineRenderer
        RaycastHit hit;
        laserDir = mouseLaserToGunPlanePoint - startPos;
        Ray ray = new(startPos, laserDir);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,startPos);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer("UI")))
            lineRenderer.SetPosition(1, hit.point);
        else
            lineRenderer.SetPosition(1, startPos + (laserDir * 100));

        //Set current aim dir for fire to be ready
        curAimDir = laserDir;

        //Set the armature rotation to face the aiming direction
        bodyArmature.transform.LookAt(transform.position + laserDir);
        bodyAnimator.SetFloat(GlobalConstants.animSpeedID, 0f);
    }

	/// <summary>
	/// Don't call this if object doesn't have a lineRenderer assigned. Sets curAimDir to a value
	/// </summary>
	void DrawLaserAim()
    {
        DrawLaserAim(out _);
    }

	/// <summary>
	/// Don't call this if object doesn't have a lineRenderer assigned. Nulls the curAimDir var
	/// </summary>
	void HideLaserAim()
	{
        if (lineRenderer == null) return;
		lineRenderer.positionCount = 0;
        aimLaserVisible = false;
        curAimDir = Vector3.zero;
	}  
    /// <summary>
	/// Don't call this if object doesn't have a lineRenderer assigned.
	/// </summary>
	void ShowLaserAim()
	{
		if (lineRenderer == null) return;
        aimLaserVisible = true;
	}
}
