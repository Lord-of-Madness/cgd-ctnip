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
	public string PromptText;
    
	
	Collider proximity;
	//[SerializeField]GameObject label;
	bool playerInProximity;

	float scaleTweenRatio = 1.3f;
	float scaleTweenDuration = 0.3f;


	private void Start()
	{
		proximity = GetComponent<Collider>();
		//label.SetActive(false);
        //GameManager.Instance.inputActions.Player.Interact.Enable();
        GameManager.Instance.inputActions.Player.Interact.performed += ctx => { if (playerInProximity) OnInteract.Invoke(); };

		//OnInteract.AddListener(TweenLabel);
		GameManager.Instance.charChanged.AddListener(HideLabel);
	}


	/*private void TweenLabel()
	{
		//label.transform.DOComplete();
		//Vector3 origScale = label.transform.localScale;
		//Debug.Log("OrigScale: "+ origScale);
		Vector3 targetScale = origScale * scaleTweenRatio;
		//Debug.Log("TargetScale: " + targetScale);
		Sequence seq = DOTween.Sequence();
		seq.SetTarget(label.transform);
		seq.Append(label.transform.DOScale(origScale * scaleTweenRatio, scaleTweenDuration/2));
		seq.Append(label.transform.DOScale(origScale, scaleTweenDuration / 2));

		//Debug.Log("Tweening label");
		seq.Play();
	}*/

	void ShowLabel()
	{
		//label.SetActive(true);
		playerInProximity = true;
        HUD.Instance.PromptLabel.text = PromptText;
    }

	void HideLabel()
	{
		//label.transform.DOComplete();
		//label.SetActive(false);
		playerInProximity = false;
        HUD.Instance.PromptLabel.text = "";
    }


	private void OnTriggerEnter(Collider other)
	{
        if (!other.isTrigger && Utilities.ActivePlayerCheck(other.gameObject))
		{
			ShowLabel();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.isTrigger && Utilities.ActivePlayerCheck(other.gameObject))
		{
			HideLabel();
		}
	}
}
