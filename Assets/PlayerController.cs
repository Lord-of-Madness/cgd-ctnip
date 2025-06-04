using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, SaveSystem.ISaveable
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
    MOVEMENT_OPTION moveOption = MOVEMENT_OPTION.cameraRelative;
    [SerializeField]
    bool controlledByPlayer = true;
    [SerializeField]
    bool isRunning = false;
    [SerializeField]
    float rotationSpeed = 8.0f;

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
    [SerializeField]
    [Tooltip("Reference to the object with camer flash -> only if the character can use such object")]
    CameraFlashScript cameraFlashScript;

    bool hasLineRenderer = false;
    public bool aimLaserVisible = false;
    Vector3 curAimDir = Vector3.zero;

    [Header("Erik")]
    [SerializeField]
    [Tooltip("Reference to the melee attack hit box script. Set only for characters with melee weapons")]
    AttackHitScript meleeAttackHitScript;
    [SerializeField]
    float meleeAttackTime = 1.5f;
    [SerializeField]
    float attackPopPartOfAnim = 0.33f;
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
    public Collider MyCollider { get; private set; }

    [Header("Sounds")]
    [SerializeField] AudioSource ToolSound;
    [SerializeField] AudioSource FootstepsSound;
    //Animation stuff
    Animator bodyAnimator;
    Quaternion desiredRotation = Quaternion.identity;

    public PlayerData playerData;
    public UnityEvent onToolUsed;
    public UnityEvent onToolSwitched;
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
    float actionCooldown = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MyCollider = transform.Find("Capsule").GetComponent<Collider>();

        if (bodyArmature != null)
        {
            bodyAnimator = bodyArmature.GetComponent<Animator>();
        }
        else { Debug.LogWarning("Armature not set! Can't save the animator component"); }
        if (cameraFlashScript == null)
        {
            Debug.LogWarning("Trying to flash with a camera, while no cameraFlashScript was set");
        }

        playerData = GetComponent<PlayerData>();

        Physics.gravity = new Vector3(0, -20, 0);

        if (controlledByPlayer) StopFollowingOtherChar();
        if (lineRenderer != null) hasLineRenderer = true;
        onToolUsed.AddListener(() =>
        {
            ToolSound.clip = playerData.SelectedTool.fireSound;
            ToolSound.Play();
        });
        onReload.AddListener(() =>
        {
            ToolSound.clip = playerData.SelectedTool.reloadSound;
            ToolSound.Play();
        });
        SaveSystem.AddSaveable(this);
    }


    public void ToggleRunningCommand()
    {
		if (controlledByPlayer) ToggleRunning();
	}

    public void ShowLaserAimCommand()
    {
		if (controlledByPlayer && actionCooldown <= 0) ShowLaserAim();
	}
    public void HideLaserAimCommand()
    {
		if (controlledByPlayer && actionCooldown <= 0) HideLaserAim();
	}
    public void AttackCommand()
    {
		if (controlledByPlayer && actionCooldown <= 0) Attack();
	}
    public void ReloadCommand()
    {
		if (controlledByPlayer && actionCooldown <= 0) Reload();
	}
    public void SwitchToolCommand()
    {
		if (controlledByPlayer && actionCooldown <= 0) SwitchTool();
	}


	private void SwitchTool()
    {
        playerData.SwitchTool();
        onToolSwitched?.Invoke();
    }

    private void Attack()
    {
        if (playerData.TryFire())
        {
            actionCooldown = playerData.SelectedTool.actionTime;
            switch (playerData.SelectedTool.toolName)
            {
                case GlobalConstants.revolverToolName: ShootFromGun(); break;
                case GlobalConstants.pipeToolName: MeleeAttack(); break;
                case GlobalConstants.cameraToolName: FlashCamera(); ; break;
                default: Debug.LogError("WHAT THE HELL DID YOU JUST USE? I have no idea what this acursed tool is!"); break;
            }
            onToolUsed.Invoke();
        }
        else
        {
            Debug.Log("Cannot use tool rn. Try reloading or aiming first");
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

        if (timeAttacking > meleeAttackTime*attackPopPartOfAnim && !dealtMeleeDamage)
            DealMeleeDamage();

        if (timeAttacking > meleeAttackTime)
            FinishMeleeAttack();

    }

    void DealMeleeDamage()
    {
        foreach (Collider c in meleeAttackHitScript.GetAllObjectsInAttackArea())
        {
            if (c.CompareTag("Enemy") && !c.isTrigger)
            {
                //Debug.Log("Applying force to an enemy!");
                //Apply force to object rigidbody (no real damage done)
                if (c.transform.parent != null)
                {
                    EnemyScript enemy = c.transform.parent.GetComponent<EnemyScript>();
                    Vector3 appliedForce = 50000 * meleeAttackForce * (enemy.transform.position - transform.position).normalized;
                    //50000 is the magic constant which make the enemy rigibody fly a bit
                    appliedForce.y = 0;

                    enemy.GetComponent<Rigidbody>().AddForce(appliedForce);
                    enemy.GetStaggered();
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
        Vector3 gunPos = transform.position +
            new Vector3(bodyArmature.transform.forward.x * weaponOffset.x,
            weaponOffset.y,
            bodyArmature.transform.forward.z * weaponOffset.x);


        Ray ray = new(gunPos, curAimDir);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.NameToLayer("UI")))
        {
            SpawnBullet(gunPos, curAimDir, 100, (transform.position - hit.point).magnitude / 100);
            if (hit.collider.CompareTag("Enemy"))
            {
                if (hit.collider.transform.parent.TryGetComponent<EnemyScript>(out var enemy)) enemy.GetHit(gunDamage);
                else Debug.Log("Collider tagged 'Enemy' didn't find EnemyScript in parent");
            }
        }
        //MISS
        else
        {
            SpawnBullet(gunPos, curAimDir, 100f, 10f);
        }

        //Try to camera shake
        if (Camera.main.TryGetComponent<CameraEffectsScript>(out var camEff))
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

    void FlashCamera()
    {
        if (bodyAnimator != null)
            bodyAnimator.SetBool(GlobalConstants.animCameraFlashID, true);
        cameraFlashScript.Flash();

    }

    private void Reload()
    {
        //TODO only shoot if aiming
        if (playerData.TryReload())//This is true only if there is a reason to actually reload - there is ammo to reload and the tool is not full
        {//The numerical changes are done in the PlayerData class
            actionCooldown = playerData.SelectedTool.reloadTime;
            onReload.Invoke();
            //TODO anima�ky and stuff
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


        if (hasLineRenderer && aimLaserVisible && controlledByPlayer)
        {
            DrawLaserAim();

        }
        actionCooldown -= Time.deltaTime;
        actionCooldown = Mathf.Max(actionCooldown, 0);
    }

    void SetAnimatorValuesMovement()
    {

        bodyAnimator.applyRootMotion = true;

        //bodyAnimator.SetBool(GlobalConstants.animGroundedID, isGrounded);
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
            desiredRotation = bodyRot.normalized;
            if (Quaternion.Angle(transform.rotation, desiredRotation) < 0.1f)
            {
                bodyArmature.transform.rotation = desiredRotation;
            }
            else
                bodyArmature.transform.rotation = Quaternion.Slerp(bodyArmature.transform.rotation, desiredRotation, rotationSpeed);
        }
    }

    void MoveByKeyboard(Vector2 dir)
    {
        if (dir == Vector2.zero)
        {
            FootstepsSound.Stop();
        }
        else if(!FootstepsSound.isPlaying) FootstepsSound.Play();
        Vector3 moveVelocity = new Vector3(dir.x, 0, dir.y) * speed;
        if (isRunning) moveVelocity *= 2;

        switch (moveOption)
        {
            case MOVEMENT_OPTION.cameraRelative:
				if (Camera.main != null)
					moveVelocity = Camera.main.transform.rotation * moveVelocity;
				else //If no camera -> basic iso movement
					moveVelocity = Quaternion.Euler(0, 45, 0) * moveVelocity;
				break;
            case MOVEMENT_OPTION.characterRelative:
                moveVelocity = bodyArmature.transform.rotation * moveVelocity;
                break;
            default:
                break;
        }

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
        if (controlledByPlayer && collision.gameObject.TryGetComponent(out InteractableScript interactableScript)&&interactableScript.commentLines.Count>0)
            HUD.Instance.PromptLabel.text = interactableScript.commentLines[0];

    }
    private void OnTriggerExit(Collider collision)
    {
        if (controlledByPlayer && collision.gameObject.TryGetComponent(out InteractableScript _))
            HUD.Instance.PromptLabel.text = "";

    }

    /*public void ShowOverheadText(List<string> lines)
    {
        overheadDialogue.ShowText(lines);
    }*/

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

        //Otherwise there is bug if switching in the middle of attack
        bodyAnimator.SetBool(GlobalConstants.animAttackID, false);
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
        //Debug.Log("Drawing laser aim");

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
        laserDir = mouseLaserToGunPlanePoint - new Vector3(bodyArmature.transform.position.x, startPos.y, bodyArmature.transform.position.z);
        Ray ray = new(startPos, laserDir);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer("UI")))
            lineRenderer.SetPosition(1, hit.point);
        else
            lineRenderer.SetPosition(1, startPos + (laserDir * 100));

        //Set current aim dir for fire to be ready
        curAimDir = laserDir;

        //Set the armature rotation to face the aiming direction
        bodyArmature.transform.LookAt(bodyArmature.transform.position + laserDir);
        bodyAnimator.SetFloat(GlobalConstants.animSpeedID, 0f);
        bodyAnimator.SetBool(GlobalConstants.animAimID, true);
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
		bodyAnimator.SetBool(GlobalConstants.animAimID, false);
	}
	/// <summary>
	/// Don't call this if object doesn't have a lineRenderer assigned.
	/// </summary>
	void ShowLaserAim()
	{
		if (lineRenderer == null) return;
        aimLaserVisible = true;
    }

	public void Save(SaveSystem.AllSavedData dataHolder)
	{
        SaveSystem.CharacterData myData = new SaveSystem.CharacterData()
        {
            name = charName,
            pos = new Vector3JsonFriendly(transform.position)
        }; 
        foreach (Tool tool in playerData.toolInspectorField)
        {
            SaveSystem.ToolData toolData = new SaveSystem.ToolData()
            {
                name = tool.toolName,
                loadedAmmo = playerData.toolInventory[tool].loadedAmmo,
                stashedAmmo = playerData.toolInventory[tool].stashedAmmo
            };
            switch (tool.toolName)
            {
                case GlobalConstants.cameraToolName:
                    myData.cameraData = toolData;
                    break;
                case GlobalConstants.revolverToolName:
                    myData.revolverData = toolData;
                    break;
                default:
                    break;
            }
        }
            
        dataHolder.charData.Add(charName, myData);
	}

	public void Load(SaveSystem.AllSavedData data)
	{
        SaveSystem.CharacterData myData = data.charData[charName];
        transform.position = myData.pos.GetVector3();

		foreach (Tool tool in playerData.toolInspectorField)
		{
			switch (tool.toolName)
			{
				case GlobalConstants.cameraToolName:
                    playerData.toolInventory[tool].loadedAmmo = myData.cameraData.loadedAmmo;
                    playerData.toolInventory[tool].stashedAmmo = myData.cameraData.stashedAmmo;
					break;
				case GlobalConstants.revolverToolName:
					playerData.toolInventory[tool].loadedAmmo = myData.revolverData.loadedAmmo;
					playerData.toolInventory[tool].stashedAmmo = myData.revolverData.stashedAmmo; break;
				default:
					break;
			}
		}

        //Restart ERIK Melee animation
        if (charName == "Erik")
        {
            bodyAnimator.SetBool(GlobalConstants.animAttackID, false);
            bodyAnimator.SetBool(GlobalConstants.animRestartId, true);
            StartCoroutine(Utilities.CallAfterSomeTime(() => bodyAnimator.SetBool(GlobalConstants.animRestartId, false), 0.5f));
        }
	}
}

public enum MOVEMENT_OPTION { cameraRelative, characterRelative }