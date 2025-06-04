using DG.Tweening;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip startGameClip;
    const float FLICKERTIME = 2f;
    public void StartGame()
    {
        Light light = audioSource.GetComponent<Light>();
        float baseintensity = light.intensity;
        DOTween.Sequence().SetEase(Ease.InSine)
            .Append(DOTween.To(() => light.intensity, x => light.intensity = x, 0, 0.01f)).AppendInterval(0.7f)
            .Append(DOTween.To(() => light.intensity, x => light.intensity = x, baseintensity/2, 0.01f)).AppendInterval(0.2f)
            .Append(DOTween.To(() => light.intensity, x => light.intensity = x, 0, 0.01f)).AppendInterval(0.2f)
            .Append(DOTween.To(() => light.intensity, x => light.intensity = x, baseintensity, 0.01f)).AppendInterval(0.2f)
            .Append(DOTween.To(() => light.intensity, x => light.intensity = x, 0, 0.01f));
        

        StartCoroutine(Utilities.CallAfterSomeTime(() => {
            audioSource.Stop();
            audioSource.PlayOneShot(startGameClip);
            StartCoroutine(Utilities.CallAfterSomeTime(() => {
                UnityEngine.SceneManagement.SceneManager.LoadScene("StartGameVoiceover", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }, startGameClip.length+2));
        }, FLICKERTIME));

        
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
