using DG.Tweening;
using DG.Tweening.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class DOTweenExtension
{
    public static Sequence TweenLight(this Sequence s,Material lightstick,Light light,Color c, float i,float interval,float speed = 0.01f)
    {
        return s.Append(DOTween.To(() => lightstick.GetColor("_EmissionColor"), x => lightstick.SetColor("_EmissionColor", x), c, speed))
                .Append(DOTween.To(() => light.intensity, x => light.intensity = x, i, speed)).AppendInterval(interval);
    }
}

public class MainMenuController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip startGameClip;
    [SerializeField] Image fadeout;
    [SerializeField] List<GameObject> buttons;
    [SerializeField] Material lightstick;
    const float FLICKERTIME = 2f;
    public float fadeoutTime = 1f;
    public void StartGame()
    {/*
        foreach (var item in lightstick.GetPropertyNames(MaterialPropertyType.Vector))
        {
            Debug.Log(item);
        }*/

        SaveSystem.RemoveAllSavedData();
        SaveSystem.CreateSaveDir();
        
        Light light = audioSource.GetComponent<Light>();
        float baseintensity = light.intensity;
        Color basecolor = lightstick.GetColor("_EmissionColor");

        DOTween.Sequence().SetEase(Ease.InSine)
            .TweenLight(lightstick, light, Color.black, 0, 0.7f)
            .TweenLight(lightstick, light, basecolor, baseintensity / 2, 0.2f)
            .TweenLight(lightstick, light, Color.black, 0, 0.2f)
            .TweenLight(lightstick, light, basecolor, baseintensity, 0.2f)
            .TweenLight(lightstick, light, Color.black, 0, 0f);
        

        StartCoroutine(Utilities.CallAfterSomeTime(() => {
            audioSource.Stop();
            audioSource.PlayOneShot(startGameClip);
            StartCoroutine(Utilities.CallAfterSomeTime(() => {
                DOTween.ToAlpha(() => fadeout.color, x => fadeout.color = x, 1, fadeoutTime);
                StartCoroutine(Utilities.CallAfterSomeTime(() => {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("StartGameVoiceover", UnityEngine.SceneManagement.LoadSceneMode.Single);
                }, fadeoutTime));
            }, startGameClip.length));
        }, FLICKERTIME));

    }
    
    public void Load()
    {
        Debug.Log("Load");
        SaveSystem.LoadLastActiveScene();
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
    public void HideButtons()
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
    }
}
