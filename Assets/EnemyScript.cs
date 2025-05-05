using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    AITarget aiTargetScript;

    int hp = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aiTargetScript = GetComponent<AITarget>();
    }

    // Update is called once per frame
    void Update()
    {
        SetAITargetToCloserChar();
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
