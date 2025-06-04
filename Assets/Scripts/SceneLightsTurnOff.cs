using UnityEngine;

public class SceneLightsTurnOff : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void TurnOffAllChildren()
    {
        foreach (Transform child in transform)
        {
            //if (child.GetComponent<Light>() != null)
                child.gameObject.SetActive(false);
        }
    }

    public void TurnOnAllChildren()
    {
		foreach (Transform child in transform)
		{
			//if (child.GetComponent<Light>() != null)
				child.gameObject.SetActive(true);
		}
	}

}
