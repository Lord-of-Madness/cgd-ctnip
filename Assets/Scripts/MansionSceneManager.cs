using UnityEngine;

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
		SaveSystem.AddGeneralSaveable(this);
	}

	private void OnDestroy()
	{
		if (disableSwap) 
			GameManager.Instance.inputActions.Player.SwapCharacters.Enable();
	}

	public void PickUpKey()
	{
		KeyPickedUp = true;
		lightsToBeTurnedOff.TurnOffAllChildren();
		foreach (var door in doorsToBeOpened) door.OpenDoor(true);
		keyObject.gameObject.SetActive(false);
		GameManager.Instance.MansionKeyPickedUp = true;
		GameManager.Instance.GramophoneGenFixed = false;
		GameManager.Instance.GramophoneSceneExternalChange = true;
	}

	public void RevertKeyPickUp()
	{
		KeyPickedUp = false;
		lightsToBeTurnedOff.TurnOnAllChildren();
		foreach (var door in doorsToBeOpened) door.CloseDoor();
		keyObject.gameObject.SetActive(true);
		
		EnemiesReleased = false;
		GameManager.Instance.MansionKeyPickedUp = false;
		GameManager.Instance.GramophoneGenFixed = true;
		GameManager.Instance.GramophoneSceneExternalChange = false;
	}

	public void SaveGeneric(SaveSystem.AllSavedData saveData)
	{
		saveData.mansionLevelData = new SaveSystem.MansionLevelData { keyPickedUp = KeyPickedUp };
	}
	public void LoadGeneric(SaveSystem.AllSavedData savedData)
	{
		//First entry of this scene
		if (savedData.mansionLevelData == null) return;
		
		if (savedData.mansionLevelData.keyPickedUp) PickUpKey();
		else RevertKeyPickUp();
	}

	public void SaveSceneSpecific(SaveSystem.AllSavedData dataHolder)
	{
		return; //All info in this manager should transition to all other scenes 
	}

	public void LoadSceneSpecific(SaveSystem.AllSavedData data)
	{
		return; //All info in this manager should transition to all other scenes 

	}
}
