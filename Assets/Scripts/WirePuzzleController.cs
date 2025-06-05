using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WirePuzzleController : MonoBehaviour
{
	[SerializeField]
	Camera myCamera;
	
	Camera cameFromCamera;

	[SerializeField]
	FuseSwitch fuseSwitch;

	OnClickMakeLine curLineDrawer;

	[SerializeField]
	bool[] matchesDone;

	[SerializeField]
	GameObject objectsParent;

	[SerializeField]
	GameObject cubesParent;

	public UnityEvent onComplete;
	public bool currentlyActive = false;
	public bool alreadyDone = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		objectsParent.SetActive(false);
		GameManager.Instance.inputActions.WirePuzzle.Click.performed += (ctx) => { if (this != null) OnClick(); };
		GameManager.Instance.inputActions.WirePuzzle.Click.canceled += (ctx) => { if (this != null) OnRelease(); };
		GameManager.Instance.inputActions.WirePuzzle.Cancel.performed += (ctx) => { if (this != null) CancelPuzzle(); };
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePuzzle()
    {
		if (alreadyDone || currentlyActive) return;

		currentlyActive = true;

		objectsParent.SetActive(true);

		GameManager.Instance.inputActions.Player.Disable();
		GameManager.Instance.inputActions.WirePuzzle.Enable();

		//Handle camera
		cameFromCamera = Camera.main;
		GameManager.Instance.DisableCameraFilter();
		cameFromCamera.enabled = false;
		myCamera.enabled = true;
		GameManager.Instance.UpdateCameraFilterState();

    }

    public void CancelPuzzle()
    {
		if (!currentlyActive) return; 
		
		currentlyActive = false;

		objectsParent.SetActive(false);

		GameManager.Instance.inputActions.Player.Enable();
		GameManager.Instance.inputActions.WirePuzzle.Disable();

		//Handle camera
		if (cameFromCamera != null)
		{
			GameManager.Instance.DisableCameraFilter();
			cameFromCamera.enabled = true;
			myCamera.enabled = false;
			GameManager.Instance.UpdateCameraFilterState();
		}

		cameFromCamera = null;

	}

	IClickable GetClickableUnderMouse()
	{
		var mousePos = Input.mousePosition;
		var mainCamera = Camera.main;

		Ray ray = mainCamera.ScreenPointToRay(mousePos);


		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.NameToLayer("Clickable")))
		{
			IClickable clickable = hit.transform.GetComponent<IClickable>();
			if (clickable != null)
			{
				return clickable;

			}
		}
		return null;
	}

	bool CheckOutOfBoundsID(int matchID)
	{
		if (matchID < 0 || matchID > matchesDone.Length)
		{
			Debug.LogWarning("MatchID incorrect -> out of bound of the control array");
			return false;
		}
		return true;
	}

	void AddMatch(int matchID)
	{
		if (!CheckOutOfBoundsID(matchID)) return;
		matchesDone[matchID] = true;

		if (CheckComplete()) Complete();
		
	}


	void RemoveMatch(int matchID)
	{
		if (!CheckOutOfBoundsID(matchID)) return;
		matchesDone[matchID] = false; 
	}

	bool CheckComplete()
	{
		for (int i = 0; i < matchesDone.Length; i++)
		{
			if (!matchesDone[i]) return false;
		}
		return true;
	}

	void Complete()
	{
		onComplete.Invoke();
		Finish();
	}

	public void Finish()
	{
		alreadyDone = true;
		this.gameObject.SetActive(false);
		CancelPuzzle();
	}

	public void UnComplete()
	{
		OnRelease(); //To cancel drawing right away
		for (int i = 0; i < matchesDone.Count(); i++)
		{
			matchesDone[i] = false;
		}
		alreadyDone = false;
		gameObject.SetActive(true);

		foreach (Transform cube in cubesParent.transform)
		{
			OnClickMakeLine lineDrawer = cube.GetComponent<OnClickMakeLine>();
			if (lineDrawer != null) lineDrawer.Cancel(true);
		}

		if (fuseSwitch.currentlyOn) fuseSwitch.OnClick();
		
	}

	void OnClick()
	{
		var clickable = GetClickableUnderMouse();
		if (clickable == null) return;
		
		if (clickable is OnClickMakeLine) BeginDraw((OnClickMakeLine)clickable);
		if (clickable is FuseSwitch) clickable.OnClick();

	}

	void BeginDraw(OnClickMakeLine clickableBox)
	{
		if (fuseSwitch.currentlyOn)//Can't do the puzzle with the light on 
		{
			//TODO: Text dialogue explaining this
			return; 
		}
		if (GameManager.Instance.activeChar == PlayerCharacter.Beth)//Beth can't do this in darkness
		{
			//TODO: Text dialogue explaining this
			return;
		}
		clickableBox.OnClick();
		curLineDrawer = clickableBox;
		RemoveMatch(curLineDrawer.matchID); //when clicking, you can cancel previous match
	}

	void OnRelease()
	{
		var finishClickable = GetClickableUnderMouse();

		//Released above another box
		if (finishClickable != null &&
			finishClickable is OnClickMakeLine &&
			curLineDrawer != null)
		{
			CancelDraw((OnClickMakeLine)finishClickable);
			return;
		}
		else if (curLineDrawer != null)
		{
			curLineDrawer.Cancel();
			curLineDrawer = null;
		}
		


		return;

	}

	void CancelDraw(OnClickMakeLine finishBox)
	{
		//Some match
		curLineDrawer.Cancel(false);

		//Correct match
		if (finishBox.matchID == curLineDrawer.matchID && finishBox != curLineDrawer)
		{
			AddMatch(curLineDrawer.matchID);
		}
		curLineDrawer = null;
		return;
	}

}
