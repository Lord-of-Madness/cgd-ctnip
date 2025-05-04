using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    int hp = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHit()
    {
        Debug.Log("I GOT HIT!! Only " +  hp + "left"); 
    }
}
