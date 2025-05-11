using UnityEngine;

public class RoomCameraSwitchOnEnter : MonoBehaviour
{
    public Camera targetCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
        //Active player check
        if (!(other.CompareTag("Player") && other.transform.parent.GetComponent<PlayerController>().IsControlledByPlayer()))
            return;

		Debug.Log("Enter room");
        if (Camera.main != null)
            Camera.main.enabled = false;
		targetCamera.enabled = true;
	}
}
