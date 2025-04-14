using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractableScript : MonoBehaviour
{
	public UnityEvent OnInteract;
    
	
	Collider proximity;
	GameObject label;
	bool playerInProximity;

	InputAction interactAction;

	float scaleTweenRatio = 1.3f;
	float scaleTweenDuration = 0.3f;


	private void Start()
	{
		proximity = GetComponent<Collider>();
		label = transform.Find("Label").gameObject;
		label.SetActive(false);

		interactAction = InputSystem.actions.FindAction("Interact");

		OnInteract.AddListener(TweenLabel);
	}


	private void Update()
	{
		if (playerInProximity && interactAction.WasPressedThisFrame())
			OnInteract.Invoke();


	}


	private void TweenLabel()
	{
		label.transform.DOComplete();
		Vector3 origScale = label.transform.localScale;
		Sequence seq = DOTween.Sequence();
		seq.Append(label.transform.DOScale(origScale * scaleTweenRatio, scaleTweenDuration/2));
		seq.Append(label.transform.DOScale(origScale, scaleTweenDuration / 2));

		//Debug.Log("Tweening label");
		seq.Play();
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
