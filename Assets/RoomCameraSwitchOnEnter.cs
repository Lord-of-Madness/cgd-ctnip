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
        if (!Utilities.ActivePlayerCheck(other.gameObject))
            return;

		Debug.Log("Enter room");
        if (Camera.main != null)
            Camera.main.enabled = false;
		targetCamera.enabled = true;
	}
}
