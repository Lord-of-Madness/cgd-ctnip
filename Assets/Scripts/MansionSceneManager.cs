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
	[SerializeField] EnemyScript scaryEnemy;
	[SerializeField] Trigger scaryTrigger;

	private void Start()
	{
		if (disableSwap)
			GameManager.Instance.inputActions.Player.SwapCharacters.Disable();
		SaveSystem.AddSaveable(this);
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
		keyObject.SetActive(false);
		GameManager.Instance.MansionKeyPickedUp = true;
		GameManager.Instance.GramophoneGenFixed = false;
		GameManager.Instance.GramophoneSceneExternalChange = true;
		scaryTrigger.gameObject.SetActive(true);
    }

	public void RevertKeyPickUp()
	{
		KeyPickedUp = false;
		lightsToBeTurnedOff.TurnOnAllChildren();
		foreach (var door in doorsToBeOpened) door.CloseDoor();
		keyObject.SetActive(true);
		
		EnemiesReleased = false;
		GameManager.Instance.MansionKeyPickedUp = false;
		GameManager.Instance.GramophoneGenFixed = true;
		GameManager.Instance.GramophoneSceneExternalChange = false;
	}

	public void Save(SaveSystem.AllSavedData saveData)
	{
		saveData.mansionLevelData = new SaveSystem.MansionLevelData { keyPickedUp = KeyPickedUp };
	}
	public void Load(SaveSystem.AllSavedData savedData)
	{
		if (savedData.mansionLevelData.keyPickedUp) PickUpKey();
		else RevertKeyPickUp();
	}
	public void ScaryMoment()
	{
		scaryEnemy.gameObject.SetActive(true);
    }
}
