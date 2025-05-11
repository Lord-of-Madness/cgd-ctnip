using DG.Tweening;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InteractableScript : MonoBehaviour
{
	public UnityEvent OnInteract;

	public List<string> commentLines = new();
    
	
	Collider proximity;
	GameObject label;
	bool playerInProximity;

	//InputAction interactAction;

	float scaleTweenRatio = 1.3f;
	float scaleTweenDuration = 0.3f;


	private void Start()
	{
		proximity = GetComponent<Collider>();
		label = transform.Find("Label").gameObject;
		label.SetActive(false);
        //GameManager.Instance.inputActions.Player.Interact.Enable();
        GameManager.Instance.inputActions.Player.Interact.performed += ctx => { if (playerInProximity) OnInteract.Invoke(); };
        //interactAction = InputSystem.actions.FindAction("Interact");

		OnInteract.AddListener(TweenLabel);
	}


	private void Update()
	{
	}


	private void TweenLabel()
	{
		int killed = label.transform.DOComplete();
		//Debug.Log("Killed: " + killed + " tweens");
		Vector3 origScale = label.transform.localScale;
		//Debug.Log("OrigScale: "+ origScale);
		Vector3 targetScale = origScale * scaleTweenRatio;
		//Debug.Log("TargetScale: " + targetScale);
		Sequence seq = DOTween.Sequence();
		seq.SetTarget(label.transform);
		seq.Append(label.transform.DOScale(origScale * scaleTweenRatio, scaleTweenDuration/2));
		seq.Append(label.transform.DOScale(origScale, scaleTweenDuration / 2));

		//Debug.Log("Tweening label");
		seq.Play();
	}



	private void OnTriggerEnter(Collider other)
	{
        PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
        if (player != null && player.IsControlledByPlayer())
		{
			label.SetActive(true);
			playerInProximity = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (Utilities.ActivePlayerCheck(other.gameObject))
		{
			label.transform.DOComplete();
			label.SetActive(false);
			playerInProximity = false;
		}
	}
}
