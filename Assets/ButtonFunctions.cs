using UnityEngine;

public class ButtonFunctions : MonoBehaviour
{
    public static void SaveGame()
    {
        SaveSystem.SaveAll();
    }

	public static void LoadGame()
    {
        SaveSystem.LoadAll();
    }

	public static void LoadAfterDeath()
    {
        GameManager.GameLoad();
    }
}
