using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField]
    string loadedSceneName = "GramofonScene";
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
        if (other.transform.parent.gameObject == GameManager.Instance.ActivePlayer.gameObject)
        {
            SceneManager.LoadScene(loadedSceneName);

        }
	}
}
