using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyScript : MonoBehaviour, SaveSystem.ISaveable
{

    AITarget aiTargetScript;
    [SerializeField]
    AttackHitScript attackZoneScript;

    [SerializeField]
    Collider myCollider;

    //[SerializeField]
    float timeToAttack = 2f;

    bool attacking = false;
    bool checkedHits = false;

    float timeAttacking = 0f;

    [SerializeField]
    float partOfAnimationToPopAttack = 0.5f;

	[SerializeField]
	float timeStaggeredAfterHit = 1f;

	bool staggered = false;

	float timeStaggered = 0f;

	[SerializeField]
    int maxHp = 10;
    int hp;

    //Animation stuff
    [SerializeField]
	Animator bodyAnimator;
    [SerializeField] AudioSource DamageDealtAudioSource;
    [SerializeField] AudioSource DamageTakenAudioSource;
    [SerializeField] AudioSource DeathAudioSource;
    [SerializeField] AudioSource SoundsAudioSource;
    [SerializeField] AudioSource FootstepsAudioSource;

    [SerializeField] List<AudioClip> Enemygrowls;

    float barkdelay;
    bool follwing=false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        hp = maxHp;

        aiTargetScript = GetComponent<AITarget>();
		bodyAnimator.SetInteger(GlobalConstants.animHpID, hp);
        barkdelay = Random.Range(2f, 10f);
        SaveSystem.AddSaveable(this);

    }

	// Update is called once per frame
	void Update()
    {
        SetAITargetToCloserChar();

        if (hp <= 0)
        {
            Die();
        }


        var animInfo = bodyAnimator.GetCurrentAnimatorStateInfo(0);
        if (attacking) {
            timeAttacking += Time.deltaTime;
            //Attack in middle -> check if anyone is hit
		    if (animInfo.fullPathHash == GlobalConstants.animAttackStateHash && animInfo.normalizedTime > partOfAnimationToPopAttack  && !checkedHits)
            {
                CheckHitsAndKill();
                timeToAttack = animInfo.length; //Update timeToAttack based on animation
		    }

            //Attack finished --> Resume following target
            if (timeAttacking > timeToAttack)
            {
                FinishAttacking();
            }
        }

        //Close enough to target --> Attack
        if ((aiTargetScript.target.position - transform.position).magnitude <= aiTargetScript.closeEnoughDistance + 0.1f && !attacking && !staggered)
        {
            Attack();
        }

        if (staggered)
        {
            timeStaggered += Time.deltaTime;
            
            //After half the stagger animation -> reset the flag so it doesn't cycle
            if (animInfo.fullPathHash == GlobalConstants.animStaggerStateHash && bodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f) 
                bodyAnimator.SetBool(GlobalConstants.animGotHitID, false);

		}

		if (timeStaggered > timeStaggeredAfterHit)
            RecoverFromStagger();

        if (follwing)
        {
            if (barkdelay > 0) barkdelay -= Time.deltaTime;
            else
            {
                Bark();
                barkdelay = Random.Range(2f, 10f);
            }
        }

    }

    void CheckHitsAndKill()
    {
		foreach (Collider c in attackZoneScript.GetAllObjectsInAttackArea())
		{
			if (c.CompareTag("Player"))
			{
                GameManager.GameOver();
			}
		}
        bodyAnimator.SetBool(GlobalConstants.animAttackID, false); //Set to false to enable trans back
        checkedHits = true;
	}

    public void StopFollowingTarget()
	{
        follwing = false;
		aiTargetScript.SetFollowing(false);
        FootstepsAudioSource.Stop();
	}

	public void ResumeFollowingTarget()
	{
        if (hp <= 0) return;
        follwing = true;
        FootstepsAudioSource.Play();
        aiTargetScript.SetFollowing(true);
        bodyAnimator.SetBool(GlobalConstants.animAttackID, false);
        bodyAnimator.SetBool(GlobalConstants.animGotHitID, false);
    }


	void Attack()
    {
		StopFollowingTarget();
		attacking = true;

		//Rotate directly to the target
		Vector3 lookPos = aiTargetScript.target.position;
		lookPos.y = transform.position.y;
		transform.LookAt(lookPos);

		//ATTACK --> TODO: Animation
		bodyAnimator.SetBool(GlobalConstants.animAttackID, true);
	}
	void FinishAttacking()
	{
        DamageDealtAudioSource.Play();
        timeAttacking = 0;
		attacking = false;
        ResumeFollowingTarget();
        checkedHits = false;

	}

    public void GetHit(int damage)
    {
        hp -= damage;
        DamageTakenAudioSource.Play();

        bodyAnimator.SetInteger(GlobalConstants.animHpID, hp);
        GetStaggered();

        //Debug.Log("I GOT HIT!! Only " +  hp + "left"); 
    }

    public void GetStaggered()
    {
        //Debug.Log("I got staggered");
		
        if (attacking) FinishAttacking();
        DamageTakenAudioSource.Play();

        staggered = true;
        bodyAnimator.SetBool(GlobalConstants.animGotHitID, true);
        StopFollowingTarget();
	}

	void RecoverFromStagger()
    {
        //Debug.Log("Recovered from stagger");
        timeStaggered = 0;
        staggered = false;
        ResumeFollowingTarget();
	}

    void SetAITargetToCloserChar()
    {
        Transform target;

        if (GameManager.Instance == null || (GameManager.Instance.bethPC == null && GameManager.Instance.erikPC == null)) return;

        if (GameManager.Instance.bethPC == null) target = GameManager.Instance.erikPC.transform;
        else if (GameManager.Instance.erikPC == null) target = GameManager.Instance.bethPC.transform;
        else if ((GameManager.Instance.erikPC.transform.position - transform.position).magnitude <= (GameManager.Instance.bethPC.transform.position - transform.position).magnitude)
        {
            target = GameManager.Instance.erikPC.transform;
        }
        else
            target = GameManager.Instance.bethPC.transform;
        aiTargetScript.target = target;
    }

    void Die()
    {
        DeathAudioSource.Play();
        StopFollowingTarget();
        //myCollider.gameObject.SetActive(false);
        enabled = false;
    }
    public void Bark()
    {
        SoundsAudioSource.PlayOneShot(Enemygrowls[Random.Range(0, Enemygrowls.Count - 1)]);
        barkdelay = Random.Range(2f, 10f);
    }

	public void Save(SaveSystem.AllSavedData dataHolder)
	{
        //If this is null when this is called -> probably memory leak in the allSaveables in SaveSystem
        SaveSystem.EnemyData myData = new SaveSystem.EnemyData
        {
            following = follwing,
			hp = hp,
            pos = new Vector3JsonFriendly(transform.position)
		};

        dataHolder.enemyData.Add(Utilities.GetFullPathName(gameObject), myData);
	}

    public void Load(SaveSystem.AllSavedData data)
    {
        //TODO: Enemy resurrection when loading from dead to alive
        SaveSystem.EnemyData myData = data.enemyData[Utilities.GetFullPathName(gameObject)];
        if (myData.following) ResumeFollowingTarget();
        else StopFollowingTarget();

        if (hp <= 0 && myData.hp > 0)
        { //Ressurect
            enabled = true;
            timeStaggered = float.MaxValue;//To resume following
        }

        hp = myData.hp;
        bodyAnimator.SetInteger(GlobalConstants.animHpID, hp);

        //Reset animation to idle
        bodyAnimator.SetBool(GlobalConstants.animRestartId, true);

        StartCoroutine(
            Utilities.CallAfterSomeTime(() => bodyAnimator.SetBool(GlobalConstants.animRestartId, false), 0.2f)
            );

        transform.position = myData.pos.GetVector3();

        }
}
