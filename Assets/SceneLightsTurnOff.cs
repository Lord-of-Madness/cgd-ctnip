using UnityEngine;

public class SceneLightsTurnOff : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void TurnOffAllChildLights()
    {
        Utilities.PurgeChildren(transform);
    }

}
