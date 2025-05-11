using UnityEngine;

public class MansionSceneManager : MonoBehaviour
{
	public static bool KeyPickedUp = false;	
	public static bool EnemiesReleased = false;

	public static void PickUpKey()
	{
		KeyPickedUp = true;
	}
}
