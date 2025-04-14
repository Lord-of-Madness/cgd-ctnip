using UnityEngine;

public class SetGlobalScale : MonoBehaviour
{
	public Vector3 fixedScale = new Vector3(1, 1, 1);
	// Update is called once per frame
	void Start()
	{
		if (transform.lossyScale != fixedScale)
			transform.localScale = new Vector3(fixedScale.x / transform.lossyScale.x, fixedScale.y / transform.lossyScale.y, fixedScale.z / transform.lossyScale.z);
	}
}
