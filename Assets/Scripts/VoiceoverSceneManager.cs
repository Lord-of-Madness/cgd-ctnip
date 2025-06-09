using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VoiceoverSceneManager : MonoBehaviour
{
    [SerializeField] Image MainImage;
    [SerializeField] List<Sprite> MainImageSprites;
    [SerializeField] List<AudioClip> Voicelines;
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ChangeImage();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MainImageSprites.Count > 0)
            {
                MainImageSprites.Clear();
                Voicelines.Clear();
                audioSource.Stop();
                DOTween.KillAll();
            }
            Trasition();
        }
    }
    void Trasition()
    {
        SceneTransitionManager.LoadNewScene("Exterior");
    }
    void ChangeImage()
    {
        if (MainImageSprites.Count > 0)
        {
            audioSource.PlayOneShot(Voicelines[0]);
            DOTween.To(() => MainImage.color, x => MainImage.color = x, Color.black, 0.2f)
                .OnComplete(() =>
                {
                    MainImage.sprite = MainImageSprites[0];
                    MainImageSprites.RemoveAt(0);
                    DOTween.To(() => MainImage.color, x => MainImage.color = x, Color.white, 0.2f);
                });
            Invoke(nameof(ChangeImage), Voicelines[0].length);
            Voicelines.RemoveAt(0);
        }
        else Trasition();
    }
}
