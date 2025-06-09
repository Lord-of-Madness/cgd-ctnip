using UnityEngine;

public class UIScript : MonoBehaviour
{
    static public UIScript Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
		if (Instance != null && Instance != this)//So it can be in multiple scenes for testing, but does not appear twice
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(Instance);
	}
}
