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

    public void InteractDoor(bool front)
    {
		openedPartReference.transform.DOKill();
        if (locked)
        {
            GameManager.Instance.ActivePlayer.ShowOverheadText(new() { "It's locked" });
            return;
        }
        if (isOpen) CloseDoor();
        else OpenDoor(front);
    }

    public void OpenDoor(bool front)
    {
        openedPartReference.transform.DOLocalRotate(new Vector3(0, front ? openAngle : -openAngle, 0), openDuration);
        isOpen = true;
    }

    public void CloseDoor()
    {
		openedPartReference.transform.DOLocalRotate(new Vector3(0, defaultAngle, 0), openDuration);
        isOpen = false;
    }

}
