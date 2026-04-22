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
    public float eyeHeight = 1.6f;
    public LayerMask obstacleMask;

    [Header("Chase")]
    public float chaseSpeed = 4f;
    public float idleSpeed = 0f;
    public float lostSightGrace = 3f;

    [Header("Repath")]
    public float repathRate = 0.25f;

    [Header("Return")]
    public Transform returnPoint;

    [Header("Tutorial")]
    public bool tutorialLocked = false;

    // Expose state for EnemyWeaponAttack to read
    public State CurrentState => state;

    private State state = State.Idle;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float nextRepath;
    private float lostSightTimer = 0f;

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

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.speed = idleSpeed;
            agent.velocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (tutorialLocked) return;
        if (!player) return;
        if (agent == null || !agent.isOnNavMesh) return;

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

        // Cast from eye level toward player's center
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 playerCenter = player.position + Vector3.up * 1f;
        Vector3 dirToPlayer = (playerCenter - eyePos).normalized;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > visionAngle / 2f) return false;

        // If raycast hits an obstacle (wall/door), can't see player
        if (Physics.Raycast(eyePos, dirToPlayer, dist, obstacleMask))
            return false;

        return true;
    }

    void EnterChase()
    {
        state = State.Chase;
        lostSightTimer = 0f;
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

        if (CanSeePlayer())
        {
            // Still sees player, reset the lost sight timer
            lostSightTimer = 0f;
        }
        else
        {
            // Lost sight — count down grace period then give up
            lostSightTimer += Time.deltaTime;
            if (lostSightTimer >= lostSightGrace)
            {
                lostSightTimer = 0f;
                EnterReturn();
            }
        }
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

    public void Unlock()
    {
        tutorialLocked = false;
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

        enabled = false;
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