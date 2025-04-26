using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AITarget : MonoBehaviour
{

    public Transform target;
    public float closeEnoughDistance = 5f;
    
    private NavMeshAgent m_agent;
    private float m_distance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_agent == null || !m_agent.enabled) { Debug.LogWarning("Agent null or disabled when trying to update AITarget!"); return; }
		if (!m_agent.isOnNavMesh) { Debug.LogWarning("Agent not on navmesh when trying to update AITarget!"); return; }
		if (!m_agent.isActiveAndEnabled) { Debug.LogWarning("Agent not active when trying to update AITarget!"); return; }
		m_distance = Vector3.Distance(m_agent.transform.position, target.position);


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
}
