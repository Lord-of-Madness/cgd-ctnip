using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AITarget : MonoBehaviour
{

    public Transform target;
    public float closeEnoughDistance = 5f;
    
    private NavMeshAgent m_agent;
    private float m_distance;
	private bool isFollowing = true;


	//Animation stuff
	[SerializeField]
	Animator bodyAnimator;
	string animSpeedID = "Speed";
	string animJumpID = "Jump";
	string animGroundedID = "Grounded";
	string animFreeFallID = "FreeFall";
	string animMotionSpeedID = "MotionSpeed";


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }


	// Update is called once per frame
	void Update()
	{
		if (m_agent == null || !m_agent.enabled) { /*Debug.LogWarning("Agent null or disabled when trying to update AITarget!");*/ return; }
		if (!m_agent.isOnNavMesh) { Debug.LogWarning("Agent not on navmesh when trying to update AITarget!"); return; }
		if (!m_agent.isActiveAndEnabled) { Debug.LogWarning("Agent not active when trying to update AITarget!"); return; }
		m_distance = Vector3.Distance(m_agent.transform.position, target.position);

		if (isFollowing)
		{
			if (m_distance < closeEnoughDistance)
			{
				m_agent.isStopped = true;
			}
			else
			{
				m_agent.isStopped = false;
				m_agent.destination = target.position;
			}
		}



		//Animations
		if (bodyAnimator != null)
		{

			//bodyAnimator.SetBool(animGroundedID, m_agent.velocity.y == 0);
			//bodyAnimator.SetBool(animJumpID, m_agent.velocity.y > 0);
			//bodyAnimator.SetBool(animFreeFallID, m_agent.velocity.y < 0);
			if (m_agent.velocity.magnitude > 0) bodyAnimator.SetFloat(animMotionSpeedID, 1);
			else bodyAnimator.SetFloat(animMotionSpeedID, 1);

			bodyAnimator.SetFloat(animSpeedID, m_agent.velocity.magnitude);
		}
		else Debug.LogWarning("Body animator is Null! Agent can't start animations");
	}

	public void SetFollowing(bool follow)
	{
		if (follow)
		{
			m_agent.enabled = follow;
			m_agent.isStopped = !follow;
		}
		if (!follow && m_agent.enabled)
		{
			m_agent.CompleteOffMeshLink();
			bodyAnimator.SetFloat(animSpeedID, 0);
			m_agent.isStopped = !follow;
			m_agent.enabled = follow;

		}
		isFollowing = follow;

	}
}
