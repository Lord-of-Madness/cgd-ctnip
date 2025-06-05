using DG.Tweening;
using UnityEngine;

public class CameraEffectsScript : MonoBehaviour
{
    [SerializeField]
    float shakeDuration = 1.0f;
	[SerializeField]
	float shakeStrength = 0.1f; 
    [SerializeField]
	int vibrato = 10;

	Camera m_camera;
    FollowPlayer m_followPlayerScript;
    bool followingChar = true;
    Sequence cameraShakeTweenSeq;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        GameManager.Instance.inputActions.Player.ToggleCameraFollow.performed += (ctx) => ToggleFollowPlayer();
        m_camera = GetComponent<Camera>();
        m_followPlayerScript = GetComponent<FollowPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ToggleFollowPlayer()
    {
        if (m_followPlayerScript == null) {
            Debug.LogWarning("Can't toggle followPlayer. No followPlayer script found in camera");
            return;
        }

        followingChar = !followingChar;
        m_followPlayerScript.enabled = followingChar;


    }

    public void CameraShake()
    {

        if (followingChar) ShakeFollowCamera();
		else ShakeStaticCamera();

    }

    void ShakeStaticCamera()
    {
		int repeats = 10;
		Vector3 origVal = m_camera.transform.position;
		cameraShakeTweenSeq = DOTween.Sequence();
		for (int i = 0; i < vibrato; i++)
		{
			cameraShakeTweenSeq.Append(DOTween.To(() => m_camera.transform.position, x => m_camera.transform.position = x
														, m_camera.transform.position + Random.insideUnitSphere * (shakeStrength), shakeDuration / (repeats + 1)));
		}
		cameraShakeTweenSeq.Append(DOTween.To(() => m_camera.transform.position, x => m_camera.transform.position = x
													, origVal, shakeDuration / (repeats + 1)));
		cameraShakeTweenSeq.Play();

	}

	void ShakeFollowCamera()
    {
        int repeats = 10;
        Vector3 origVal = m_followPlayerScript.offsetFromPlayer;
		cameraShakeTweenSeq = DOTween.Sequence();
        for (int i = 0; i < vibrato; i++)
        {
            cameraShakeTweenSeq.Append(DOTween.To(() => m_followPlayerScript.offsetFromPlayer, x => m_followPlayerScript.offsetFromPlayer = x
                                                        , m_followPlayerScript.offsetFromPlayer + Random.insideUnitSphere*(shakeStrength), shakeDuration/(repeats+1)));
        }
		cameraShakeTweenSeq.Append(DOTween.To(() => m_followPlayerScript.offsetFromPlayer, x => m_followPlayerScript.offsetFromPlayer = x
													,origVal , shakeDuration / (repeats+1)));
        cameraShakeTweenSeq.Play();
	}
}
