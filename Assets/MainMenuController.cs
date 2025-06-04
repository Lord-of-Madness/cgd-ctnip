using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip startGameClip;
    public void StartGame()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(startGameClip);
        Debug.Log(startGameClip.length);
        StartCoroutine(Utilities.CallAfterSomeTime(() => {
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartGameVoiceover", UnityEngine.SceneManagement.LoadSceneMode.Single);
        },startGameClip.length));
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
