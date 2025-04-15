using UnityEngine;

[ExecuteInEditMode]
public class FreezeGlobalRotation : MonoBehaviour
{
    public Vector3 fixedRotation = new Vector3(30,45,0);
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = fixedRotation;
    }
}
