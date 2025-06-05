using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public Vector3 offsetFromPlayer = new Vector3(50,50,10);
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player is not null)
            transform.position = player.transform.position + offsetFromPlayer;
    }
}
