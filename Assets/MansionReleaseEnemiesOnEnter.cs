using NUnit.Framework;
using UnityEngine;

public class MansionReleaseEnemiesOnEnter : MonoBehaviour
{
	[SerializeField]
	EnemyScript[] enemiesToBeReleased;
	private void OnTriggerEnter(Collider other)
	{
		if (!Utilities.ActivePlayerCheck(other.gameObject))
			return;

		if (!(MansionSceneManager.KeyPickedUp && !MansionSceneManager.EnemiesReleased))
			return;

		MansionSceneManager.EnemiesReleased = true;

		foreach (EnemyScript enemy in enemiesToBeReleased) { 
			enemy.ResumeFollowingTarget();
		}
	}
}
