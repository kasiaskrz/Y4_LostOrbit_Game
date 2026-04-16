using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chase,
        GoToCover,
        InCover,
        Reposition
    }

    [Header("Refs")]
    public NavMeshAgent agent;
    public Animator anim;
    public Transform player;
    public Transform muzzle;

    [Header("Detection")]
    public float detectRange = 25f;
    public float combatRange = 16f;   // when we start thinking about cover
    public float tooCloseRange = 4f;  // leave cover if player pushes

    [Header("Cover")]
    public float coverSearchRadius = 18f;
    public float coverArriveDistance = 1.1f;
    public float coverMinDistanceFromPlayer = 6f; // don't pick cover right beside player
    public string coverPointTag = "CoverPoint";

    [Header("Popout / Shooting")]
    public float popoutCooldownMin = 1.2f;
    public float popoutCooldownMax = 2.5f;
    public float shootRange = 30f;

    [Header("Repath")]
    public float repathRate = 0.25f;

    State state;
    Transform currentCover;
    float nextRepath;
    float nextPopoutTime;

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!anim) anim = GetComponentInChildren<Animator>();

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        // Play spawn once
        anim.SetTrigger("Spawn");
        SetCover(false);

        state = State.Idle;
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Animator movement bool
        anim.SetBool("IsMoving", agent.velocity.magnitude > 0.1f);

        // If player far, idle
        if (dist > detectRange)
        {
            state = State.Idle;
            SetAgentStopped(true);
            SetCover(false);
            currentCover = null;
            return;
        }

        // If player is too close and we're in cover, leave and reposition
        if (dist < tooCloseRange && state == State.InCover)
        {
            LeaveCoverAndReposition();
            return;
        }

        switch (state)
        {
            case State.Idle:
                // If player detected, start chase
                state = State.Chase;
                break;

            case State.Chase:
                HandleChase(dist);
                break;

            case State.GoToCover:
                HandleGoToCover(dist);
                break;

            case State.InCover:
                HandleInCover(dist);
                break;

            case State.Reposition:
                HandleReposition(dist);
                break;
        }
    }

    void HandleChase(float dist)
    {
        // If within combat range, try find cover
        if (dist <= combatRange)
        {
            Transform cover = FindBestCoverPoint();
            if (cover)
            {
                currentCover = cover;
                state = State.GoToCover;

                // Trigger enter cover animation + bool
                anim.SetTrigger("EnterCover");
                SetCover(true);

                SetAgentStopped(false);
                agent.stoppingDistance = coverArriveDistance;
                agent.SetDestination(currentCover.position);
                return;
            }
        }

        // Otherwise chase normally
        SetAgentStopped(false);
        agent.stoppingDistance = 1.8f;

        if (Time.time >= nextRepath && agent.isOnNavMesh)
        {
            nextRepath = Time.time + repathRate;
            agent.SetDestination(player.position);
        }
    }

    void HandleGoToCover(float dist)
    {
        if (!currentCover)
        {
            SetCover(false);
            state = State.Chase;
            return;
        }

        SetAgentStopped(false);

        if (Time.time >= nextRepath && agent.isOnNavMesh)
        {
            nextRepath = Time.time + repathRate;
            agent.SetDestination(currentCover.position);
        }

        // Arrived
        if (!agent.pathPending && agent.remainingDistance <= coverArriveDistance + 0.15f)
        {
            SetAgentStopped(true);
            state = State.InCover;

            // schedule first popout
            nextPopoutTime = Time.time + Random.Range(popoutCooldownMin, popoutCooldownMax);
        }
    }

    void HandleInCover(float dist)
    {
        if (!currentCover)
        {
            SetCover(false);
            state = State.Chase;
            return;
        }

        SetAgentStopped(true);
        FacePlayer();

        // Popout & shoot
        if (Time.time >= nextPopoutTime)
        {
            nextPopoutTime = Time.time + Random.Range(popoutCooldownMin, popoutCooldownMax);

            anim.SetTrigger("Popout");
            ShootRaycast();
        }

        // If player moved far away, leave cover and chase
        if (dist > combatRange + 6f)
        {
            ExitCoverToChase();
        }
    }

    void HandleReposition(float dist)
    {
        // Once we've moved a bit, go back to chase/cover logic
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            state = State.Chase;
        }
    }

    void LeaveCoverAndReposition()
    {
        // Play exit cover animation
        anim.SetTrigger("ExitCover");
        SetCover(false);

        currentCover = null;

        // Move away from player
        Vector3 away = (transform.position - player.position).normalized;
        Vector3 target = transform.position + away * 7f;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 6f, NavMesh.AllAreas))
        {
            SetAgentStopped(false);
            agent.stoppingDistance = 0f;
            agent.SetDestination(hit.position);
            state = State.Reposition;
        }
        else
        {
            state = State.Chase;
        }
    }

    void ExitCoverToChase()
    {
        anim.SetTrigger("ExitCover");
        SetCover(false);
        currentCover = null;

        SetAgentStopped(false);
        agent.stoppingDistance = 1.8f;
        state = State.Chase;
    }

    void SetCover(bool value)
    {
        anim.SetBool("InCover", value);
    }

    // Safe wrapper — prevents "agent not on NavMesh" errors
    void SetAgentStopped(bool stopped)
    {
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = stopped;
    }

    Transform FindBestCoverPoint()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag(coverPointTag);
        if (points == null || points.Length == 0) return null;

        Transform best = null;
        float bestScore = float.NegativeInfinity;

        foreach (var go in points)
        {
            Transform t = go.transform;

            float distToEnemy = Vector3.Distance(transform.position, t.position);
            if (distToEnemy > coverSearchRadius) continue;

            float distToPlayer = Vector3.Distance(player.position, t.position);
            if (distToPlayer < coverMinDistanceFromPlayer) continue;

            // Simple score: prefer closer cover to enemy, but not too close to player
            float score = 0f;
            score += (coverSearchRadius - distToEnemy);     // closer is better
            score += Mathf.Clamp(distToPlayer, 0f, 30f) * 0.15f; // farther from player slightly better

            // (Optional) prefer cover that is "between" enemy and player less (avoid open spots)
            // You can add LOS checks later if you want.

            if (score > bestScore)
            {
                bestScore = score;
                best = t;
            }
        }

        return best;
    }

    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
    }

    void ShootRaycast()
    {
        if (!muzzle) return;

        Ray ray = new Ray(muzzle.position, muzzle.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 0.2f);
            // Later: apply damage if hit player
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * shootRange, Color.red, 0.2f);
        }
    }
}