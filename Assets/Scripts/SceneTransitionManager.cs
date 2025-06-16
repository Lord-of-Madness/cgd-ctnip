using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{

	public static SceneTransitionManager Instance { get; set; }
	private void Awake()
	{
		if (Instance != null && Instance != this)//So it can be in multiple scenes for testing, but does not appear twice
		{
			//Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(Instance);
	}

	public static void LoadNewScene(string sceneName)
	{
		if (SaveSystem.activeInScene)
		{
			SaveSystem.SaveAll();
			Instance.StartCoroutine(LoadNewSceneAndThenLoadSave(sceneName));
		}
		else
			SceneManager.LoadScene(sceneName);
	}

	private static IEnumerator LoadNewSceneAndThenLoadSave(string sceneName)
	{
		var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
		while (!asyncLoadLevel.isDone)
		{
			Debug.Log("Loading the Scene");
			yield return null;
		}

        SaveSystem.LoadAll();
        Debug.Log("Called load in the new scene");
        yield break;
	}
}
