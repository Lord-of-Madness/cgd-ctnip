using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField]
    string loadedSceneName = "GramofonScene";
	public static SceneTransition Instance { get; set; }
	private void Awake()
	{
		if (Instance != null && Instance != this)//So it can be in multiple scenes for testing, but does not appear twice
		{
			Instance.loadedSceneName = this.loadedSceneName;
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(Instance);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
        //if (other == null || GameManager.Instance.ActivePlayer == null) return; //probably popped in between loading
        if (other.transform.parent.gameObject == GameManager.Instance.ActivePlayer.gameObject)
        {
            SaveSystem.Save();
            StartCoroutine(LoadNewSceneAndThenLoadSave(loadedSceneName));
        }
	}
	private static IEnumerator LoadNewSceneAndThenLoadSave(string sceneName)
	{
		var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
		while (!asyncLoadLevel.isDone)
		{
			Debug.Log("Loading the Scene");
			yield return null;
		}

        SaveSystem.Load();
        Debug.Log("Called load in the new scene");
        yield break;
	}
}
