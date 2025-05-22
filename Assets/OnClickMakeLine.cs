using UnityEngine;

public class OnClickMakeLine : MonoBehaviour, IClickable
{
	LineRenderer lineRenderer;

	bool drawingLine = false;

	public int matchID = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		lineRenderer = GetComponent<LineRenderer>();

	}

	// Update is called once per frame
	void Update()
    {
        if (drawingLine) DrawUpdate();
    }

	public void OnClick()
	{
		StartDrawingLine();
	}

	public void OnRelease()
	{
		Cancel();
	}

	public void StartDrawingLine()
	{
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition(0, transform.position);
		drawingLine = true;

	}

	void DrawUpdate()
	{
		//Get the position of mouse direciton intersecion with the plane of the gun
		Vector3 mouseDir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
		Vector3 mouseToBoxPlanePoint;
		if (!Utilities.LinePlaneIntersection(out mouseToBoxPlanePoint, Camera.main.transform.position, mouseDir, transform.rotation*Vector3.back, transform.position))
			mouseToBoxPlanePoint = transform.position + Vector3.forward;
		lineRenderer.SetPosition(1, mouseToBoxPlanePoint);
	}

    public void Cancel(bool removeLine = true)
    {
		if (removeLine) lineRenderer.positionCount = 0;
		
		drawingLine=false;
    }

}
