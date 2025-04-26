using Unity.AI.Navigation.Samples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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

    SpriteRenderer spriteRenderer;

    [Header("References")]
    [SerializeField]
    OverheadDialogue overheadDialogue;

    InputAction moveAction;
    InputAction jumpAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		// 3. Find the references to the "Move" and "Jump" actions
		moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();

        Physics.gravity = new Vector3(0, -20, 0);
    }

    // Update is called once per frame
    void Update()
    {
		// 4. Read the "Move" action value, which is a 2D vector
		// and the "Jump" action state, which is a boolean value

		Vector2 moveValue = moveAction.ReadValue<Vector2>();
		MoveByKeyboard(moveValue);


        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            Jump();
        }
        else timeSinceJump += Time.deltaTime;

        if (controlledByPlayer)
            Move();
        
        if (curVelocity.y > 0) curVelocity.y -= (Time.deltaTime/jumpTime) * jumpForce;

	}

    void Move()
    {
		//Debug.Log("Moving in dir: " + curVelocity.ToString());
        transform.position += curVelocity * Time.deltaTime;
	}

	void MoveByKeyboard(Vector2 dir)
    {
        Vector3 moveVelocity = new Vector3(dir.x, 0, dir.y) * speed;

        if (isoMovement) moveVelocity = Quaternion.Euler(0,45,0) * moveVelocity;

        curVelocity.x = moveVelocity.x;
        curVelocity.z = moveVelocity.z;
    }

    void Jump()
    {
        timeSinceJump = 0;
        //Debug.Log("Fire jump");
        curVelocity.y = jumpForce;
        isGrounded = false;
    }


    //Called from FeetCollider
    public void FeetTriggerStay()
    {
        //Debug.Log("Feet collision detected");
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

    public void DisablePlayerControl()
    {
        controlledByPlayer = false;
    }

    public void EnablePlayerControl()
    {
        controlledByPlayer = true;
    }
}
