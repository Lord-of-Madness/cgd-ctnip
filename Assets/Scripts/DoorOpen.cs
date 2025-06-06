using DG.Tweening;
using UnityEngine;

public class DoorOpen : MonoBehaviour, SaveSystem.ISaveable
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

	private void Start()
	{
	    SaveSystem.AddSaveable(this);	
	}

	public void InteractDoor(bool front)
    {
		openedPartReference.transform.DOKill();
        if (locked)
        {
            HUD.Instance.PromptLabel.text = "It's locked";
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

    public void Lock() => locked = true;
    public void Unlock() => locked = false;

	public void Save(SaveSystem.AllSavedData dataHolder)
	{
		dataHolder.doorData.Add(Utilities.GetFullPathName(gameObject), new SaveSystem.DoorData() {isOpen = isOpen});
	}

	public void Load(SaveSystem.AllSavedData data)
	{
		bool incomingIsOpen = data.doorData[Utilities.GetFullPathName(gameObject)].isOpen;


        if (incomingIsOpen) OpenDoor(true);
        else CloseDoor();
	}
}
