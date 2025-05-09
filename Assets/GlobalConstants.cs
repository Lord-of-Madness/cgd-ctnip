using System.Numerics;
using UnityEditor.Search;
using UnityEngine;

public static class GlobalConstants
{
	//ANIMATIONS
	public static string animSpeedID = "Speed";
	public static string animJumpID = "Jump";
	public static string animGroundedID = "Grounded";
	public static string animFreeFallID = "FreeFall";
	public static string animMotionSpeedID = "MotionSpeed";
	public static string animAttackID = "Attack";
	public static string animGotHitID = "GotHit";
	public static string animHpID = "HP";
	public static string animAimID = "IsAiming";

	public static int animAttackStateHash = Animator.StringToHash("Base Layer.Attack");
	public static int animStaggerStateHash = Animator.StringToHash("Base Layer.GetHit");

	//TOOLS
	public static string pipeToolName = "Pipe";
	public static string cameraToolName = "Camera flash";
	public static string revolverToolName = "Revolver";
}
