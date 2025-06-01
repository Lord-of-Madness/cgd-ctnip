using UnityEngine;

public class RoomCameraSwitchOnEnter : MonoBehaviour
{
    public Camera targetCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.charChanged.AddListener(CheckActivePlayerInside);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
        if (!Utilities.ActivePlayerCheck(other.gameObject))
            return;

        SwapToThisCamera();
	}

    void SwapToThisCamera()
	{
		if (Camera.main != null)
		{
			GameManager.Instance.DisableCameraFilter();
            Camera.main.GetComponent<AudioListener>().enabled = false;
			Camera.main.enabled = false;

        }
        targetCamera.GetComponent<AudioListener>().enabled = true;
		targetCamera.enabled = true;
        GameManager.Instance.UpdateCameraFilterState();

	}

    void CheckActivePlayerInside()
    {
        //BIG FAT CHEAT TO TRIGGER THE ON_TRIGGER
        GameManager.Instance.ActivePlayer.MyCollider.enabled = false;
        GameManager.Instance.ActivePlayer.MyCollider.enabled = true;
    }
}
