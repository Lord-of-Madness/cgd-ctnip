using UnityEngine;


public class SetGlobalScale : MonoBehaviour
{
	public Vector3 fixedScale = new Vector3(1, 1, 1);
	// Update is called once per frame
	void OnEnable()
	{
		//Debug.Log("Global scale = " + transform.lossyScale.ToString());
		if (transform.lossyScale != fixedScale)
		{
			Vector3 globalIfLocal1 = new Vector3(transform.lossyScale.x/transform.localScale.x, transform.lossyScale.y / transform.localScale.y, transform.lossyScale.z / transform.localScale.z);
			//Debug.Log("Setting local scale to :" + (fixedScale.x / globalIfLocal1.x) + " , " + (fixedScale.y / globalIfLocal1.y) + " , " + (fixedScale.z / globalIfLocal1.z));
			transform.localScale = new Vector3(fixedScale.x / globalIfLocal1.x, fixedScale.y / globalIfLocal1.y, fixedScale.z / globalIfLocal1.z);
		}
	}
}
