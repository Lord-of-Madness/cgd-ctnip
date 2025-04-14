using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractableScript : MonoBehaviour
{
    Collider proximity;
	GameObject label;
	bool playerInProximity;

	public UnityEvent OnInteract;

	private void Start()
	{
		proximity = GetComponent<Collider>();
		label = transform.Find("Label").gameObject;
		label.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			label.SetActive(true);
			playerInProximity = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			label.SetActive(false);
			playerInProximity = false;
		}
	}
}
