using UnityEngine;
using UnityEngine.AI;

public class EnemyRoomAttacker : MonoBehaviour
{
    [Header("Movement")]
    public NavMeshAgent agent;
    public Transform targetRoom;
    public bool keepUpdatingDestination = true;
    public float updateRate = 0.25f;

    private float updateTimer;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null && targetRoom != null)
        {
            agent.SetDestination(targetRoom.position);
        }
    }

    void Update()
    {
        if (agent == null || targetRoom == null) return;

        if (!keepUpdatingDestination) return;

        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0f)
        {
            updateTimer = updateRate;
            agent.SetDestination(targetRoom.position);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        targetRoom = newTarget;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null && targetRoom != null)
        {
            agent.SetDestination(targetRoom.position);
        }
    }
}