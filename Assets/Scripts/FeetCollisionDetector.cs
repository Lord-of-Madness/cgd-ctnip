using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FeetCollisionDetector : MonoBehaviour
{
    public UnityEvent feetTriggerStay;
    public UnityEvent feetTriggerExit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

	private void OnTriggerStay(Collider other)
	{
        if (other.isTrigger || other.CompareTag("Player")) return;
        //Debug.Log("Trigger feet stay!");
        feetTriggerStay.Invoke();
    }

	private void OnTriggerExit(Collider other)
	{
        //Debug.Log("Trigger feet exit!");
        feetTriggerExit.Invoke();
	}
}
