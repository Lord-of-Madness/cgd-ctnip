using DG.Tweening;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{

    [SerializeField]
    float openAngle = 90f;
    [SerializeField]
    float defaultAngle = 0;
    [SerializeField]
    float openDuration = 0.5f;

    bool isOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InteractDoor()
    {
        transform.DOKill();
        if (isOpen) CloseDoor();
        else OpenDoor();
    }

    void OpenDoor()
    {
        transform.DORotate(new Vector3(0, openAngle, 0), openDuration);
        isOpen = true;
    }

    void CloseDoor()
    {
        transform.DORotate(new Vector3(0, defaultAngle, 0), openDuration);
        isOpen = false;
    }

}
