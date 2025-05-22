using UnityEngine;

public class MansionSceneManager : MonoBehaviour
{
	public static bool KeyPickedUp = false;	
	public static bool EnemiesReleased = false;
	public bool disableSwap = true;

	private void Start()
	{
		if (disableSwap)
			GameManager.Instance.inputActions.Player.SwapCharacters.Disable();
	}

	public static void PickUpKey()
	{
		KeyPickedUp = true;
	}
}
