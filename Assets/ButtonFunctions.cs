using Unity.VisualScripting;
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
    public static void QuitGame() {
        SaveSystem.SaveAll();
        Application.Quit();
    }
}
