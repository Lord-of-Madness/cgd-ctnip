using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WirePuzzleController : MonoBehaviour
{
	[SerializeField]
	Camera myCamera;
	
	Camera cameFromCamera;
	
	OnClickMakeLine curLineDrawer;

	[SerializeField]
	bool[] matchesDone;

	public UnityEvent onComplete;
	bool alreadyDone = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		GameManager.Instance.inputActions.WirePuzzle.Click.performed += (ctx) => { BeginDraw(); };
		GameManager.Instance.inputActions.WirePuzzle.Click.canceled += (ctx) => { CancelDraw(); };
		GameManager.Instance.inputActions.WirePuzzle.Cancel.performed += (ctx) => { CancelPuzzle(); };
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePuzzle()
    {
		if (alreadyDone) return;

		GameManager.Instance.inputActions.Player.Disable();
		GameManager.Instance.inputActions.WirePuzzle.Enable();
		cameFromCamera = Camera.main;
		cameFromCamera.enabled = false;
		myCamera.enabled = true;
		GameManager.Instance.UpdateCameraFilterState();

    }

    public void CancelPuzzle()
    {
		GameManager.Instance.inputActions.Player.Enable();
		GameManager.Instance.inputActions.WirePuzzle.Disable();
		cameFromCamera.enabled = true;
		myCamera.enabled = false;
		Debug.Log("CanceledPuzzle");
	}

	OnClickMakeLine GetBoxUnderMouse()
	{
		var mousePos = Input.mousePosition;
		var mainCamera = Camera.main;

		Ray ray = mainCamera.ScreenPointToRay(mousePos);


		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.NameToLayer("Clickable")))
		{
			OnClickMakeLine clickableBox = hit.transform.GetComponent<OnClickMakeLine>();
			if (clickableBox != null)
			{
				return clickableBox;

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
		matchesDone[curLineDrawer.matchID] = true;

		if (CheckComplete()) Complete();
		
	}


	void RemoveMatch(int matchID)
	{
		if (!CheckOutOfBoundsID(matchID)) return;
		matchesDone[curLineDrawer.matchID] = false; 
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
		alreadyDone = true;
		CancelPuzzle();
	}

	void BeginDraw()
	{
		var clickableBox = GetBoxUnderMouse();
		if (clickableBox == null) return;

		clickableBox.StartDrawingLine();
		curLineDrawer = clickableBox;
		RemoveMatch(curLineDrawer.matchID); //when clicking, you can cancel previous match
	}

	void CancelDraw()
	{
		if (curLineDrawer == null) return;

		var finishBox = GetBoxUnderMouse();

		//No match
		if (finishBox == null)
		{
			curLineDrawer.Cancel();
			curLineDrawer = null;
			return;
		}

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
