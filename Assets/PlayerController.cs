using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float jumpForce = 10f;
    [SerializeField]
    float speed = 100;

    InputAction moveAction;
    InputAction jumpAction;
    Rigidbody rb;

    Vector3 curVelocity = Vector3.zero;

    bool isGrounded = true;
    float timeSinceJump = 0;
    [SerializeField]
    float minTimeBetweenJumps = 0.5f;
    [SerializeField]
    float jumpTime = 2;

    [SerializeField]
    bool isoMovement = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		// 3. Find the references to the "Move" and "Jump" actions
		moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");


        rb = GetComponent<Rigidbody>();

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

        Move();
        
        if (curVelocity.y > 0) curVelocity.y -= (Time.deltaTime/jumpTime) * jumpForce;

	}

    void Move()
    {
		Debug.Log("Moving in dir: " + curVelocity.ToString());
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
        Debug.Log("Fire jump");
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

}
