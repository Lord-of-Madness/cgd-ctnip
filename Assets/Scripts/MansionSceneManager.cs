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
	[SerializeField] GameObject blockadeOfTransition;
	[SerializeField] TextAsset KeyDialog;

	bool ScaryTriggerTriggered = false;
	bool ScareHappened = false;

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

		SaveSystem.RemoveGenSaveable(this);
	}
	public void OnFlashed()
	{
		if (ScareHappened)
		{
			scaryEnemy.gameObject.SetActive(false);
        }
		if (ScaryTriggerTriggered)
		{
			ScaryTriggerTriggered = false;
			scaryEnemy.PlayCackle();

            ScareHappened = true;
        }
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
		blockadeOfTransition.SetActive(false);
		Dialogue.Instance.ShowCharacterWithText(DialogueTreeNode.DeserializeTree(KeyDialog) );
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
		blockadeOfTransition.SetActive(true);
	}

	public void SaveGeneric(SaveSystem.AllSavedData saveData)
	{
		saveData.mansionLevelData = new SaveSystem.MansionLevelData { keyPickedUp = KeyPickedUp };
	}
	public void LoadGeneric(SaveSystem.AllSavedData savedData)
	{
		//First entry of this scene
		if (savedData.mansionLevelData == null) return;

		if (savedData.mansionLevelData.keyPickedUp)
		{
			if (savedData.gramophoneLevelData.generatorFixed) //If generator already fixed -> only delete the keyItemObject
			{
				EnemiesReleased = true;
				lightsToBeTurnedOff.TurnOnAllChildren();
				KeyPickedUp = true;
				keyObject.SetActive(false);
				blockadeOfTransition.SetActive(false);
			}
			else
				PickUpKey();
		}
		else RevertKeyPickUp();
	}
	public void ScaryMoment()
	{
		scaryEnemy.gameObject.SetActive(true);
        ScaryTriggerTriggered = true;
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
