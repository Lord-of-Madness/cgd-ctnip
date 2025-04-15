using Unity.VisualScripting;
using UnityEngine;
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

    Vector3 curVelocity = Vector3.zero;

    [Header("CharacterSprites")]
    [SerializeField]
    Sprite char1Sprite;
    [SerializeField]
    Sprite char2Sprite;

    SpriteRenderer spriteRenderer;

    [Header("References")]
    [SerializeField]
    OverheadDialogue overheadDialogue;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction swapCharAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		// 3. Find the references to the "Move" and "Jump" actions
		moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        swapCharAction = InputSystem.actions.FindAction("SwapCharacters");

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

        Move();
        
        if (curVelocity.y > 0) curVelocity.y -= (Time.deltaTime/jumpTime) * jumpForce;

        if (swapCharAction.WasPressedThisFrame())
        {
            GameManager.Instance.SwapCharacters();
            AssignCharSprite();
        }

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

    void AssignCharSprite()
    {
        if (GameManager.Instance.firstCharacterActive)
        {
            spriteRenderer.sprite = char1Sprite;
        }
        else
        {
            spriteRenderer.sprite = char2Sprite;
        }
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
}
