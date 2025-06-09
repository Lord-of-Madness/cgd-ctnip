using DG.Tweening;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light lightSource;
    [SerializeField] Material lightstick;
    public bool flickerEnabled;
    public float cooldown = 0;
    public float Darktime=0;
    public float Lighttime=0;
    void Start()
    {
        if (flickerEnabled)
        {
            float baseintensity = lightSource.intensity;
            Color basecolor = lightstick.GetColor("_EmissionColor");
            DOTween.Sequence().SetEase(Ease.InSine)
                .TweenLight(lightstick, lightSource, Color.black, 0, 0.2f).AppendInterval(Darktime)
                .TweenLight(lightstick, lightSource, basecolor, baseintensity / 2, 0.2f).AppendInterval(Lighttime)
                .TweenLight(lightstick, lightSource, Color.black, 0, 0.2f).AppendInterval(Darktime)
                .TweenLight(lightstick, lightSource, basecolor, baseintensity, 0.2f).AppendInterval(Lighttime)
                .TweenLight(lightstick, lightSource, Color.black, 0, 0f).AppendInterval(Darktime)
                .TweenLight(lightstick, lightSource, basecolor, baseintensity, 0.2f).AppendInterval(Lighttime)
                .AppendInterval(cooldown)
                .SetLoops(9999);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
