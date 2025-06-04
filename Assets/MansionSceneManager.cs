using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class MansionSceneManager : MonoBehaviour, SaveSystem.ISaveable
{
	public GameObject keyObject;
	public static bool KeyPickedUp = false;	
	public static bool EnemiesReleased = false;
	public bool disableSwap = true;

	[SerializeField]
	SceneLightsTurnOff lightsToBeTurnedOff;

	[SerializeField]
	DoorOpen[] doorsToBeOpened;

	private void Start()
	{
		if (disableSwap)
			GameManager.Instance.inputActions.Player.SwapCharacters.Disable();
		SaveSystem.AddSaveable(this);
	}

	public void PickUpKey()
	{
		KeyPickedUp = true;
		lightsToBeTurnedOff.TurnOffAllChildren();
		foreach (var door in doorsToBeOpened) door.OpenDoor(true);
		keyObject.gameObject.SetActive(false);
	}

	public void RevertKeyPickUp()
	{
		KeyPickedUp = false;
		lightsToBeTurnedOff.TurnOnAllChildren();
		foreach (var door in doorsToBeOpened) door.CloseDoor();
		keyObject.gameObject.SetActive(true);
	}

	public void Save(SaveSystem.AllSavedData saveData)
	{
		saveData.mansionLevelData = new SaveSystem.MansionLevelData { keyPickedUp = KeyPickedUp };
	}
	public void Load(SaveSystem.AllSavedData savedData)
	{
		if (savedData.mansionLevelData.keyPickedUp && !KeyPickedUp) PickUpKey();
		else if (!savedData.mansionLevelData.keyPickedUp && KeyPickedUp) RevertKeyPickUp();
	}
}
