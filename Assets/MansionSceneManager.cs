using UnityEngine;
using UnityEngine.InputSystem;

public class MansionSceneManager : MonoBehaviour, SaveSystem.ISaveable
{
	public InteractableScript keyObject;
	public static bool KeyPickedUp = false;	
	public static bool EnemiesReleased = false;
	public bool disableSwap = true;

	private void Start()
	{
		SaveSystem.AddSaveable(this);
		if (disableSwap)
			GameManager.Instance.inputActions.Player.SwapCharacters.Disable();
	}

	public static void PickUpKey()
	{
		KeyPickedUp = true;
	}

	public void Save(SaveSystem.AllSavedData saveData)
	{
		saveData.mansionLevelData = new SaveSystem.MansionLevelData { keyPickedUp = KeyPickedUp };
	}
	public void Load(SaveSystem.AllSavedData savedData)
	{
		if (savedData.mansionLevelData.keyPickedUp && !KeyPickedUp) keyObject.OnInteract.Invoke();
	}
}
