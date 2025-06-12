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
    [SerializeField]
    string lockedText = "It's locked";

    bool isOpen = false;

	private void Start()
	{
	    SaveSystem.AddSceneSaveable(this);	
	}

	public void InteractDoor(bool front)
    {
		openedPartReference.transform.DOKill();
        if (locked)
        {
            HUD.Instance.PromptLabel.text = lockedText;
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

	public void SaveSceneSpecific(SaveSystem.AllSavedData dataHolder)
	{
		dataHolder.doorData.Add(Utilities.GetFullPathName(gameObject), new SaveSystem.DoorData() {isOpen = isOpen});
	}

	public void LoadSceneSpecific(SaveSystem.AllSavedData data)
	{
        if (!data.doorData.ContainsKey(Utilities.GetFullPathName(gameObject))) return;
		
        bool incomingIsOpen = data.doorData[Utilities.GetFullPathName(gameObject)].isOpen;


        if (incomingIsOpen) OpenDoor(true);
        else CloseDoor();
	}

	public void SaveGeneric(SaveSystem.AllSavedData dataHolder)
	{
        return; //All doors are only scene specific
	}

	public void LoadGeneric(SaveSystem.AllSavedData data)
	{
		return; //All doors are only scene specific
	}
}
