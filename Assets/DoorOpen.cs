using DG.Tweening;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    [SerializeField]
    GameObject openedPartReference;

    [SerializeField]
    float openAngle = 90f;
    [SerializeField]
    float defaultAngle = 0;
    [SerializeField]
    float openDuration = 0.5f;
    [SerializeField]
    bool locked = false;

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
		openedPartReference.transform.DOKill();
        if (locked)
        {
            GameManager.Instance.ActivePlayer.ShowOverheadText(new() { "It's locked" });
            return;
        }
        if (isOpen) CloseDoor();
        else OpenDoor();
    }

    public void OpenDoor()
    {
        openedPartReference.transform.DOLocalRotate(new Vector3(0, openAngle, 0), openDuration);
        isOpen = true;
    }

    public void CloseDoor()
    {
		openedPartReference.transform.DOLocalRotate(new Vector3(0, defaultAngle, 0), openDuration);
        isOpen = false;
    }

}
