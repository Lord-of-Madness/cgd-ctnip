using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    AITarget aiTargetScript;
    [SerializeField]
    AttackHitScript attackZoneScript;

    [SerializeField]
    float timeToAttack = 1f;

    bool attacking = false;
    bool checkedHits = false;

    float timeAttacking = 0f;

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


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        hp = maxHp;

        aiTargetScript = GetComponent<AITarget>();
		bodyAnimator.SetInteger(GlobalConstants.animHpID, hp);

	}

	// Update is called once per frame
	void Update()
    {
        SetAITargetToCloserChar();


		if (attacking) timeAttacking += Time.deltaTime;

		//Attack in middle -> check if anyone is hit
		if (timeAttacking > timeToAttack/3 && !checkedHits)
        {
            CheckHitsAndKill();
		}
        //Attack finished --> Resume following target
        if (timeAttacking > timeToAttack)
        {
            FinishAttacking();
        }

        //Close enough to target --> Attack
        if ((aiTargetScript.target.position - transform.position).magnitude <= aiTargetScript.closeEnoughDistance + 0.1f && !attacking && !staggered)
        {
            Attack();
        }

		if (staggered) timeStaggered += Time.deltaTime;

        if (timeStaggered > timeStaggeredAfterHit)
            RecoverFromStagger();


    }

    void CheckHitsAndKill()
    {
		foreach (Collider c in attackZoneScript.GetAllObjectsInAttackArea())
		{
			if (c.CompareTag("Player"))
			{
				//TODO: Actual kill of the player and end of game
				Debug.LogWarning("ONE OF THE CHARACTERS HIT!! GAME OVER!! YOU ARE DEAD!!! HAHAH!!");
			}
		}
		checkedHits = true;
	}

    void StopFollowingTarget()
	{
		aiTargetScript.SetFollowing(false);

	}

	void ResumeFollowingTarget()
	{
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
		timeAttacking = 0;
		attacking = false;
        ResumeFollowingTarget();
        checkedHits = false;

	}

    public void GetHit(int damage)
    {
        hp -= damage;

        bodyAnimator.SetInteger(GlobalConstants.animHpID, hp);
        GetStaggered();

        Debug.Log("I GOT HIT!! Only " +  hp + "left"); 
    }

    public void GetStaggered()
    {
        Debug.Log("I got staggered");
		
        if (attacking) FinishAttacking();
        
        staggered = true;
		bodyAnimator.SetBool(GlobalConstants.animGotHitID, true);
        StopFollowingTarget();
	}

	void RecoverFromStagger()
    {
        Debug.Log("Recovered from stagger");
        timeStaggered = 0;
        staggered = false;
        ResumeFollowingTarget();
	}

    void SetAITargetToCloserChar()
    {
        Transform target = GameManager.Instance.bethPC.transform;
        if ((GameManager.Instance.erikPC.transform.position - transform.position).magnitude <= (GameManager.Instance.bethPC.transform.position - transform.position).magnitude) { 
            target = GameManager.Instance.erikPC.transform;
        }
        aiTargetScript.target = target;
    }
}
