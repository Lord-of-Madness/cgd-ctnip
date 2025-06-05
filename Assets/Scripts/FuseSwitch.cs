using DG.Tweening;
using UnityEngine;

public class FuseSwitch : MonoBehaviour, IClickable
{

    [SerializeField]
    Vector3 posOn;

	[SerializeField]
    Vector3 posOff;

    [SerializeField]
    float flickDuration = 0.15f;

    [SerializeField]
    GameObject fuseLight;

    public bool currentlyOn = false;

    Tween moveTween;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (moveTween.IsActive()) return;
        
        currentlyOn = !currentlyOn;

        if (currentlyOn)
            moveTween = transform.DOLocalMove(posOn, flickDuration).SetEase(Ease.InOutCubic);
        else
			moveTween = transform.DOLocalMove(posOff, flickDuration).SetEase(Ease.InOutCubic);

		fuseLight.SetActive(currentlyOn);
    }

    public void OnRelease() { }
}
