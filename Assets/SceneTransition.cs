using UnityEngine;

public class SceneTransition : MonoBehaviour
{
	[SerializeField]
	string loadedSceneName = "GramofonScene";

	bool readyToTrans = false;

	private void Start()
	{
		//Wait for 2 secs after scene load beforea transitioning back
		StartCoroutine(Utilities.CallAfterSomeTime(() => readyToTrans = true, 2));
	}


	private void OnTriggerEnter(Collider other)
	{
		if (!readyToTrans) return;
		//if (other == null || GameManager.Instance.ActivePlayer == null) return; //probably popped in between loading
		if (Utilities.ActivePlayerCheck(other.gameObject))
		{
			SceneTransitionManager.LoadNewScene(loadedSceneName);
		}
	}
}
