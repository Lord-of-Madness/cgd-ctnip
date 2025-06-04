using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartGameVoiceover", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public void Load()
    {
        Debug.Log("Load");
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
