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


    int hp = 100;

    //Animation stuff
    [SerializeField]
	Animator bodyAnimator;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        aiTargetScript = GetComponent<AITarget>();
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
            ResumeFollowingTarget();
        }

        //Close enough to target --> Attack
        if ((aiTargetScript.target.position - transform.position).magnitude <= aiTargetScript.closeEnoughDistance + 0.1f && !attacking)
        {
            Attack();
        }



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

    void ResumeFollowingTarget()
	{
		timeAttacking = 0;
		attacking = false;
		aiTargetScript.SetFollowing(true);
		bodyAnimator.SetBool(GlobalConstants.animAttackID, false);
		checkedHits = false;

	}

    void Attack()
    {
		//Stop following target
		aiTargetScript.SetFollowing(false);
		attacking = true;

		//Rotate directly to the target
		Vector3 lookPos = aiTargetScript.target.position;
		lookPos.y = transform.position.y;
		transform.LookAt(lookPos);

		//ATTACK --> TODO: Animation
		bodyAnimator.SetBool(GlobalConstants.animAttackID, true);
	}

    public void GetHit()
    {
        Debug.Log("I GOT HIT!! Only " +  hp + "left"); 
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
