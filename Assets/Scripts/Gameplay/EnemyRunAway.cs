using UnityEngine;
using UnityEngine.AI;

public class EnemyFleeNavmesh : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // Drag your player here
    public BoxCollider arenaBounds;   // Drag your ArenaBounds here

    [Header("Flee Settings")]
    public float fleeTriggerDistance = 6f;  // Start fleeing if closer than this
    public float fleeDistance = 8f;         // How far we try to move away
    public float repathInterval = 0.3f;     // How often we update the path

    private NavMeshAgent agent;
    private float nextRepathTime = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null || arenaBounds == null || agent == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only compute a new flee target every repathInterval
        if (distanceToPlayer < fleeTriggerDistance && Time.time >= nextRepathTime)
        {
            // Direction away from player on XZ plane
            Vector3 awayDir = (transform.position - player.position);
            awayDir.y = 0f;

            if (awayDir.sqrMagnitude < 0.01f)
            {
                // If we're basically on top of the player, just pick some direction
                awayDir = transform.forward;
            }

            awayDir.Normalize();

            // Raw flee target (straight away from player)
            Vector3 target = transform.position + awayDir * fleeDistance;

            // Clamp that target to arena bounds so we don't leave the map
            target = ClampToBoundsXZ(target, arenaBounds.bounds);

            // Project target onto NavMesh
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            nextRepathTime = Time.time + repathInterval;
        }
    }

    private Vector3 ClampToBoundsXZ(Vector3 pos, Bounds b)
    {
        pos.x = Mathf.Clamp(pos.x, b.min.x, b.max.x);
        pos.z = Mathf.Clamp(pos.z, b.min.z, b.max.z);
        // Keep current Y so we don't force ground height
        return pos;
    }
}
