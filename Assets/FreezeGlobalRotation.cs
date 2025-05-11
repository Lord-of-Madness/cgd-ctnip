using UnityEngine;

[ExecuteInEditMode]
public class FreezeGlobalRotation : MonoBehaviour
{
    [SerializeField]
    bool setBasedOnCamera = true;

    public Vector3 fixedRotation = new Vector3(30,45,0);
    // Update is called once per frame
    void Update()
    {
        if(setBasedOnCamera && Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;
        else
            transform.eulerAngles = fixedRotation;

    }
}
