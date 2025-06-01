using UnityEngine;

public static class GlobalConstants
{
	//ANIMATIONS
	public const string animSpeedID = "Speed";
	public const string animJumpID = "Jump";
	public const string animGroundedID = "Grounded";
	public const string animFreeFallID = "FreeFall";
	public const string animMotionSpeedID = "MotionSpeed";
	public const string animAttackID = "Attack";
	public const string animGotHitID = "GotHit";
	public const string animHpID = "HP";
	public const string animAimID = "IsAiming";
	public const string animCameraFlashID = "TakePhoto";

	public static int animAttackStateHash = Animator.StringToHash("Base Layer.Attack");
	public static int animStaggerStateHash = Animator.StringToHash("Base Layer.GetHit");
	public static int animIdleStateHash = Animator.StringToHash("Base Layer.Idle Walk Run Blend");

	//TOOLS
	public const string pipeToolName = "Pipe";
	public const string cameraToolName = "Camera flash";
	public const string revolverToolName = "Revolver";

	//CHAR_NAMES
	public const string bethName = "Beth";
	public const string erikName = "Erik";



	//SAVING
	public static string savePath = Application.persistentDataPath + "/save.json";
}
