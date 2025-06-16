using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static PlayerData;

public class PlayerController : MonoBehaviour, SaveSystem.ISaveable
{
    [Header("CharacterName")]
    public string charName = "Beth";
    /*
    [Header("Jumping")]
    [SerializeField]
    float jumpForce = 10f;
    [SerializeField]
    float minTimeBetweenJumps = 0.5f;
    [SerializeField]float jumpTime = 2;
    */
    bool isGrounded = true;
    float timeSinceJump = 0;

    [Header("Movement")]
    [SerializeField]
    float speed = 100;
    public
    MOVEMENT_OPTION moveOption = MOVEMENT_OPTION.cameraRelative;
    [SerializeField]
    bool controlledByPlayer = true;
    [SerializeField]
    bool isRunning = false;
    [SerializeField]
    float rotationSpeed = 0.8f;
    [SerializeField]
    float rotationSpeedTankControls = 0.035f;
    bool tankControlsReverseMovement = false;

    Vector3 curVelocity = Vector3.zero;

    [Header("Combat")]
    [Header("Beth")]
    [SerializeField]
    [Tooltip("This is an offset of the gun when held in hand. Set only if the character holds a gun. X = horizontal, Y = vertical")]
    Vector2 weaponOffset = new(0.2f, 1f);
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
    public CharacterController MyCharController { get; private set; }

    [Header("Sounds")]
    [SerializeField] AudioSource ToolSound;
    [SerializeField] AudioSource FootstepsSound;
    public AudioSource VoiceSource;
    [SerializeField] AudioClip OutOfAmmo;
    [SerializeField] AudioClip HaveToAim;
    [SerializeField] AudioClip NoToolSelected;
    [SerializeField] AudioClip NopeCantDoThat;
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
    internal bool unlessIFuckingWantTo = false;
    public AITarget aITarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aITarget = GetComponent<AITarget>();
        //MyCollider = transform.Find("Capsule").GetComponent<Collider>();
        if (GameManager.Instance.tankControls) { moveOption = MOVEMENT_OPTION.characterRelative; }
        else { moveOption = MOVEMENT_OPTION.cameraRelative; }
        MyCharController = GetComponent<CharacterController>();

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

        Physics.gravity = new(0, -20, 0);

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
        SaveSystem.AddSceneSaveable(this);
        SaveSystem.AddGeneralSaveable(this);
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
		if (controlledByPlayer) HideLaserAim();
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
        VoiceSource.PlayOneShot(playerData.SelectedTool.equipSound);
        onToolSwitched?.Invoke();
    }

    private void Attack()
    {
        ToolUseExcuses toolUseExcuses = playerData.TryFire();
        if (toolUseExcuses == ToolUseExcuses.AllClear)
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
            switch (toolUseExcuses)
            {
                case ToolUseExcuses.OutOfAmmo:
                    Debug.Log("Out of ammo!");
                    VoiceSource.PlayOneShot(OutOfAmmo);
                    break;
                case ToolUseExcuses.GottaAim:
                    Debug.Log("Gotta aim first!");
                    VoiceSource.PlayOneShot(HaveToAim);
                    break;
                case ToolUseExcuses.NoToolSelected:
                    Debug.Log("No tool selected!"); VoiceSource.PlayOneShot(NoToolSelected);
                    break;
                default:
                    Debug.LogError("Unknown tool use excuse: " + toolUseExcuses);
                    break;
            }
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
        if (playerData.TryReload())//This is true only if there is a reason to actually reload - there is ammo to reload and the tool is not full
        {//The numerical changes are done in the PlayerData class
            actionCooldown = playerData.SelectedTool.reloadTime;
            onReload.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 4. Read the "Move" action value, which is a 2D vector
        // and the "Jump" action state, which is a boolean value

        Vector2 moveValue = GameManager.Instance.inputActions.Player.Move.ReadValue<Vector2>();
        if (moveOption == MOVEMENT_OPTION.cameraRelative) MoveByKeyboard(moveValue);
        else MoveByKeyBoardTank(moveValue);

        SetVerticalVelocity();

        timeSinceJump += Time.deltaTime;

        if (controlledByPlayer)
            MeleeAttackTimingManagment();


        if (controlledByPlayer && !aimLaserVisible && !meleeAttacking)
        {
            Move();

            if (moveOption == MOVEMENT_OPTION.cameraRelative) RotateInMoveDir();
            else RotateTank(moveValue);

            SetAnimatorValuesMovement();

        }
        else if (!controlledByPlayer)
        {
            bodyAnimator.applyRootMotion = false;
        }


        if (hasLineRenderer && aimLaserVisible && controlledByPlayer)
        {
            DrawLaserAim();

        }
        actionCooldown -= Time.deltaTime;
        actionCooldown = Mathf.Max(actionCooldown, 0);
    }

	private void OnDestroy()
	{
        //Debug.Log("Destroying character....");
        SaveSystem.RemoveGenSaveable(this);
	}

	void SetAnimatorValuesMovement()
    {
        Vector3 horizontalVelocity = new(curVelocity.x, 0, curVelocity.z);

        horizontalVelocity /= transform.localScale.x; //Because velocity is scaled with the model -> animator is still the same

        bodyAnimator.applyRootMotion = true;

        //bodyAnimator.SetBool(GlobalConstants.animGroundedID, isGrounded);
        bodyAnimator.SetBool(GlobalConstants.animJumpID, curVelocity.y > 0);

        if (horizontalVelocity.magnitude > 0) bodyAnimator.SetFloat(GlobalConstants.animMotionSpeedID, 1);
        else bodyAnimator.SetFloat(GlobalConstants.animMotionSpeedID, 1);


        if (moveOption == MOVEMENT_OPTION.characterRelative && tankControlsReverseMovement)
		    bodyAnimator.SetFloat(GlobalConstants.animSpeedID, -horizontalVelocity.magnitude * 1);
        else
            bodyAnimator.SetFloat(GlobalConstants.animSpeedID, horizontalVelocity.magnitude * 1);
    }
    void Move()
    {
        //Debug.Log("Moving in dir: " + curVelocity.ToString());
        //transform.position += curVelocity * Time.deltaTime;

        //CharaController version
        // move the player

		MyCharController.Move(curVelocity * Time.deltaTime);

	}

    void RotateInMoveDir()
    {
        if (!controlledByPlayer) { bodyArmature.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); return; }

		Vector3 horizontalVelocity = new(curVelocity.x, 0, curVelocity.z);

		//Rotate in direction of velocity
		if (horizontalVelocity != Vector3.zero)
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
            {
                switch (moveOption)
                {
                    case MOVEMENT_OPTION.cameraRelative:
                        bodyArmature.transform.rotation = Quaternion.Slerp(bodyArmature.transform.rotation, desiredRotation, rotationSpeed);
                        break;
                    case MOVEMENT_OPTION.characterRelative:
						bodyArmature.transform.rotation = Quaternion.Slerp(bodyArmature.transform.rotation, desiredRotation, rotationSpeedTankControls);
						break;
                    default:
                        break;
                }
            }
        }
    }

    void RotateTank(Vector2 dir)
    {
		if (!controlledByPlayer) { bodyArmature.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); return; }


		//Rotate in direction of keyboard input
		if (dir.x != 0)
		{
			Quaternion bodyRot = bodyArmature.transform.rotation * Quaternion.Euler(new Vector3(0, dir.x, 0).normalized*rotationSpeedTankControls);


            //Debug.Log("Rotating body to " + bodyRot.ToString() + " and curVelocity is: " + curVelocity);
            desiredRotation = bodyRot.normalized;
			if (Quaternion.Angle(transform.rotation, desiredRotation) < 0.1f)
			{
				bodyArmature.transform.rotation = desiredRotation;
			}
			else
			{
			    bodyArmature.transform.rotation = Quaternion.Slerp(bodyArmature.transform.rotation, desiredRotation, rotationSpeedTankControls);
			}
		}
	}

    void MoveByKeyboard(Vector2 dir)
    {
        if (dir == Vector2.zero)
        {
            FootstepsSound.Stop();
        }
        else if(!FootstepsSound.isPlaying) FootstepsSound.Play();

        Vector3 moveVelocity = new Vector3(dir.x, 0, dir.y).normalized;

		if (Camera.main != null)
			moveVelocity = Quaternion.Euler(0,Camera.main.transform.rotation.eulerAngles.y,0) * moveVelocity;
		else //If no camera -> basic iso movement
			moveVelocity = Quaternion.Euler(0, 45, 0) * moveVelocity;

        moveVelocity = moveVelocity.normalized * speed * transform.localScale.x; //Local scale for scene scaled up
        if (isRunning) moveVelocity *= 2;

        curVelocity.x = moveVelocity.x;
        curVelocity.z = moveVelocity.z;
	}

    void MoveByKeyBoardTank(Vector2 dir)
    {
        Vector3 moveVelocity = new Vector3(0,0, dir.y).normalized;

        //Going backwards
        if (moveVelocity.z < 0) tankControlsReverseMovement = true;
        else tankControlsReverseMovement = false;

		moveVelocity = bodyArmature.transform.rotation * moveVelocity;

        moveVelocity = speed * transform.localScale.x * moveVelocity.normalized;
		if (isRunning) moveVelocity *= 2;


            curVelocity.x = moveVelocity.x;
        curVelocity.z = moveVelocity.z;
	}

	void SetVerticalVelocity()
    {
		//Vertical velocity
		if (isGrounded)
			curVelocity.y = -2;
        else curVelocity.y += Physics.gravity.y * Time.deltaTime;

        if (curVelocity.y < -20) curVelocity.y = -20f;
	}

    /// <summary>
    /// This has to be done after each transform.position change -> otherwise CharacterController resets the position back
    /// </summary>
    void SetCharControllerPosition(Vector3 pos)
    {
        MyCharController.enabled = false;
        MyCharController.transform.position = pos;
        MyCharController.enabled = true;
    }

    void ToggleRunning()
    {
        isRunning = !isRunning;
    }
    /*
    void Jump()
    {
        timeSinceJump = 0;
        //Debug.Log("Fire jump");
        curVelocity.y = jumpForce;
        isGrounded = false;

        bodyAnimator.SetBool(GlobalConstants.animJumpID, true);

    }*/

    /*
    //Called from FeetCollider
    public void FeetTriggerStay()
    {
        //Debug.Log("Feet collision detected in " + charName);
        //This is because collider can collider the next frame after jump and instantly enable jumping again
        if (timeSinceJump > minTimeBetweenJumps)
            isGrounded = true;
    }*/

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
        aITarget.SetFollowing(false);
        //gameObject.GetComponent<AgentLinkMover>().enabled = false;
    }

    public void StartFollowingOtherChar()
    {
        //gameObject.GetComponent<NavMeshAgent>().enabled = true;
        aITarget.SetFollowing(true);
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
        if (bodyAnimator != null)
            bodyAnimator.SetBool(GlobalConstants.animAttackID, false);
    }

    public void EnablePlayerControl()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        controlledByPlayer = true;
    }

    /// <summary>
    /// Don't call this if object doesn't have a lineRenderer assigned. Sets curAimDir to a value
    /// </summary>
    Vector3 DrawLaserAim()
    {
        Vector3 laserDir;
        //Debug.Log("Drawing laser aim");

        //Get the position of the gun
        Vector3 startPos = transform.position;
        startPos += new Vector3(bodyArmature.transform.forward.x * weaponOffset.x, weaponOffset.y, bodyArmature.transform.forward.z * weaponOffset.x);

        //Get the position of mouse direciton intersecion with the plane of the gun
        Vector3 mouseDir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        if (!Utilities.LinePlaneIntersection(out Vector3 mouseLaserToGunPlanePoint, Camera.main.transform.position, mouseDir, Vector3.up, startPos))
            mouseLaserToGunPlanePoint = startPos + Vector3.forward;

        //Draw the line via saved lineRenderer
        laserDir = mouseLaserToGunPlanePoint - new Vector3(bodyArmature.transform.position.x, startPos.y, bodyArmature.transform.position.z);
        Ray ray = new(startPos, laserDir);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.NameToLayer("UI")))
            lineRenderer.SetPosition(1, hit.point);
        else
            lineRenderer.SetPosition(1, startPos + (laserDir * 100));

        //Set current aim dir for fire to be ready
        curAimDir = laserDir;

        //Set the armature rotation to face the aiming direction
        bodyArmature.transform.LookAt(bodyArmature.transform.position + laserDir);
        bodyAnimator.SetFloat(GlobalConstants.animSpeedID, 0f);
        bodyAnimator.SetBool(GlobalConstants.animAimID, true);
        return laserDir;
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

        if (bodyAnimator != null)
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

	public void SaveGeneric(SaveSystem.AllSavedData dataHolder)
	{
        SaveSystem.CharacterGenData myData = new()
        {
            HP = playerData.HP,
            moveOption = moveOption,
            Documents = playerData.Documents,
            Codex = playerData.Codex,
            Inventory = playerData.Inventory
        };

        foreach (Tool tool in playerData.toolInspectorField)
        {
            SaveSystem.ToolData toolData = new()
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

        if (!dataHolder.charGenData.ContainsKey(charName))
            dataHolder.charGenData.Add(charName, myData);
        else
            dataHolder.charGenData[charName] = myData;
	}

	public void LoadGeneric(SaveSystem.AllSavedData data)
	{
        SaveSystem.CharacterGenData myData = data.charGenData[charName];

        playerData.HP = myData.HP;
        moveOption = myData.moveOption;

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

        playerData.Documents = new(myData.Documents);
        playerData.Codex = new(myData.Codex);
        playerData.Inventory = new(myData.Inventory);

        //Restart ERIK Melee animation
        if (charName == "Erik")
        {
            bodyAnimator.SetBool(GlobalConstants.animAttackID, false);
            bodyAnimator.SetBool(GlobalConstants.animRestartId, true);
            StartCoroutine(Utilities.CallAfterSomeTime(() => bodyAnimator.SetBool(GlobalConstants.animRestartId, false), 0.5f));
        }
	}


	public void SaveSceneSpecific(SaveSystem.AllSavedData dataHolder)
	{
		SaveSystem.CharacterSceneData myData = new()
		{
			name = charName,
			pos = new Vector3JsonFriendly(transform.position)
		};

		dataHolder.charSceneData.Add(charName, myData);
	}

	public void LoadSceneSpecific(SaveSystem.AllSavedData data)
	{
		SaveSystem.CharacterSceneData myData = data.charSceneData[charName];
		SetCharControllerPosition(myData.pos.GetVector3());

		//Restart ERIK Melee animation
		if (charName == "Erik")
		{
			bodyAnimator.SetBool(GlobalConstants.animAttackID, false);
			bodyAnimator.SetBool(GlobalConstants.animRestartId, true);
			StartCoroutine(Utilities.CallAfterSomeTime(() => bodyAnimator.SetBool(GlobalConstants.animRestartId, false), 0.5f));
		}
	}


    internal void PlayNope()
    {
        VoiceSource.PlayOneShot(NopeCantDoThat);
    }
}

public enum MOVEMENT_OPTION { cameraRelative, characterRelative }