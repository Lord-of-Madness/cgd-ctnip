using UnityEngine;

public class SceneTransition : MonoBehaviour
{
	[SerializeField]
	string loadedSceneName = "GramofonScene";


	private void OnTriggerEnter(Collider other)
	{
		//if (other == null || GameManager.Instance.ActivePlayer == null) return; //probably popped in between loading
		if (Utilities.ActivePlayerCheck(other.gameObject))
		{
			SceneTransitionManager.LoadNewScene(loadedSceneName);
		}
	}
}
