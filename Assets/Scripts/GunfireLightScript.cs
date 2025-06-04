using DG.Tweening;
using UnityEngine;

public class GunfireLightScript : MonoBehaviour
{
	[SerializeField]
	float exposureTime = 0.5f;
	[SerializeField]
	float flashIntesity = 400;
	[SerializeField]
	float flashRange = 20;
	[SerializeField]
	Ease tweenEase = Ease.Linear;
	[SerializeField]
	Vector2 offset = Vector2.zero;

	[Header("References")]
	[SerializeField]
	Transform transformRef;
	[SerializeField]
	PlayerController playerController;

	Light gunfireLight;

	Tween flashTween;
	bool flashing = false;

	private void Awake()
	{
		playerController.onToolUsed.AddListener(Flash);
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		gunfireLight = GetComponent<Light>();
	}

	// Update is called once per frame
	void Update()
	{
		transform.position = transformRef.position +
				new Vector3(transformRef.forward.x * offset.x,
				offset.y,
				transformRef.forward.z * offset.x);
	}


	public void Flash()
	{
		//Debug.Log("Gunfire flash called");
		if (flashing || (flashTween != null && flashTween.active)) return;

		//Check if the used tool is actually a gun -> could be checked elsewhere, but idk how rn
		if (playerController.playerData.SelectedTool.toolName != GlobalConstants.revolverToolName) return;

		gunfireLight.intensity = flashIntesity;
		gunfireLight.range = flashRange;
		flashTween = gunfireLight.DOIntensity(0, exposureTime).SetEase(tweenEase);
		flashing = true;
		flashTween.OnComplete(() => flashing = false);
	}
}
