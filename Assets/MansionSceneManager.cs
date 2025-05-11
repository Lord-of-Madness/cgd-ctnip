using UnityEngine;

public class MansionSceneManager : MonoBehaviour
{
	public static bool KeyPickedUp = false;	
	public static bool EnemiesReleased = false;


	private void Start()
	{
		GameManager.Instance.inputActions.Player.SwapCharacters.Disable();
	}

	public static void PickUpKey()
	{
		KeyPickedUp = true;
	}
}
