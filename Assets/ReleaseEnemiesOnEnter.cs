using NUnit.Framework;
using UnityEngine;

public class ReleaseEnemiesOnEnter : MonoBehaviour
{
	[SerializeField]
	EnemyScript[] enemiesToBeReleased;
	private void OnTriggerEnter(Collider other)
	{
		if (!Utilities.ActivePlayerCheck(other.gameObject))
			return;

		foreach (EnemyScript enemy in enemiesToBeReleased) { 
			enemy.ResumeFollowingTarget();
		}
	}
}
