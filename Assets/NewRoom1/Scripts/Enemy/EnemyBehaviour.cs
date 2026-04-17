using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public enum State { Idle, Chase, Return }

    [Header("References")]
    public NavMeshAgent agent;
    public Animator anim;
    public Transform player;

    [Header("Detection")]
    public float visionRange = 15f;
    public float visionAngle = 90f;
    public LayerMask obstacleMask;

    [Header("Chase")]
    public float chaseSpeed = 4f;
    public float idleSpeed = 0f;

    [Header("Repath")]
    public float repathRate = 0.25f;

    [Header("Return")]
    public Transform returnPoint; // assign in Inspector, leave empty to use start position

    private State state = State.Idle;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float nextRepath;

    void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponentInChildren<Animator>();

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (returnPoint != null)
            startPosition = returnPoint.position;
        else if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            startPosition = hit.position;
        else
            startPosition = transform.position;

        startRotation = transform.rotation;

        agent.isStopped = true;
        agent.speed = idleSpeed;
        agent.velocity = Vector3.zero;
    }

    void Update()
    {
        if (!player) return;

        switch (state)
        {
            case State.Idle:
                if (CanSeePlayer())
                    EnterChase();
                break;

            case State.Chase:
                HandleChase();
                break;

            case State.Return:
                HandleReturn();
                break;
        }
    }

    bool CanSeePlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > visionRange) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > visionAngle / 2f) return false;

        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, dist, obstacleMask))
            return false;

        return true;
    }

    void EnterChase()
    {
        state = State.Chase;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
    }

    void HandleChase()
    {
        if (Time.time >= nextRepath)
        {
            nextRepath = Time.time + repathRate;
            agent.SetDestination(player.position);
        }

        float dist = Vector3.Distance(transform.position, player.position);


        if (!CanSeePlayer() && dist > visionRange)
            EnterReturn();
    }

    void EnterReturn()
    {
        state = State.Return;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.ResetPath();
        agent.SetDestination(startPosition);
    }

    void HandleReturn()
    {
        float distToStart = Vector3.Distance(transform.position, startPosition);

        if (distToStart <= 1.1f)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();

            // Stop rigidbody if present
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * 5f);

            if (Quaternion.Angle(transform.rotation, startRotation) < 1f)
            {
                transform.rotation = startRotation;
                state = State.Idle;
            }
        }
    }


    public void Die()
    {
        state = State.Idle;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        enabled = false; // stops Update() completely
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftDir = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftDir * visionRange);
        Gizmos.DrawRay(transform.position, rightDir * visionRange);

    }
}