using System;
using System.Collections;
using UnityEngine;
public class Utilities
{
	public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
	{
		float length;
		float dotNumerator;
		float dotDenominator;
		Vector3 vector;
		intersection = Vector3.zero;

		//calculate the distance between the linePoint and the line-plane intersection point
		dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
		dotDenominator = Vector3.Dot(lineVec, planeNormal);

		if (dotDenominator != 0.0f)
		{
			length = dotNumerator / dotDenominator;

			vector = lineVec.normalized * length;

			intersection = linePoint + vector;

			return true;
		}

		else
			return false;
	}
    public static void PurgeChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }

	public static bool ActivePlayerCheck(GameObject potentialPlayer)
	{
		return potentialPlayer.CompareTag("Player") && potentialPlayer.transform.parent.GetComponent<PlayerController>().IsControlledByPlayer();
			
	}

	public static string GetFullPathName(GameObject obj)
	{
		string path = "/" + obj.name + obj.transform.GetSiblingIndex();
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			path = "/" + obj.name + obj.transform.GetSiblingIndex() + path;
		}
		return path;
	}

	public static IEnumerator CallAfterSomeTime(Action action, float time)
	{
		yield return new WaitForSeconds(time);
		action.Invoke();
		yield break;
	}
}
