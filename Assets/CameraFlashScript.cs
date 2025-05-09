using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class CameraFlashScript : MonoBehaviour
{
    [SerializeField]
    float exposureTime = 0.5f;
    [SerializeField]
    float flashIntesity = 400;
    [SerializeField]
    float flashRange = 20;
    [SerializeField]
    Ease tweenEase = Ease.Linear;

    [Header("References")]
    [SerializeField]
    Transform rotationTransformRef;

	Light flashLight;

    Tween flashTween;
    bool flashing = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flashLight = GetComponent<Light>();
	}

	// Update is called once per frame
	void Update()
    {
        transform.rotation = rotationTransformRef.rotation;
    }


    public void Flash()
    {
        if (flashing || (flashTween != null && flashTween.active)) return;


        flashLight.intensity = flashIntesity;
        flashLight.range = flashRange;
        flashTween = flashLight.DOIntensity(0, exposureTime).SetEase(tweenEase);
        flashing = true;
        flashTween.OnComplete(() => flashing = false);
    }
}
