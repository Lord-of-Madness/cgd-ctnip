using UnityEngine;
using UnityEngine.SceneManagement;

public class VoiceoverSceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        Invoke(nameof(Trasition), audioSource.clip.length+3);
    }

    void Trasition()
    {
        SceneManager.LoadScene("Exterior", LoadSceneMode.Single);
    }
}
