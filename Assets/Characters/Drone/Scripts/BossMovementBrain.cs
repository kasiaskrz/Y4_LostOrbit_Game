using UnityEngine;

public class BossMovementBrain : MonoBehaviour
{
    public enum MoveMode
    {
        Hold,
        Orbit,
        Retreat,
        Reposition
    }

    [Header("References")]
    public Transform player;
    public BossMovement movement;
    public BoxCollider arenaBounds;

    [Header("Decision Timing")]
    public float thinkInterval = 1.2f;
    public float stuckCheckTime = 2.5f;

    [Header("Distance Rules")]
    public float tooCloseDistance = 8f;
    public float idealMinDistance = 12f;
    public float idealMaxDistance = 20f;
    public float tooFarDistance = 26f;

    [Header("Movement")]
    public float orbitStepDistance = 8f;
    public float retreatDistance = 12f;
    public float repositionDistance = 14f;

    [Header("Reposition")]
    [Range(0f, 1f)] public float repositionChance = 0.15f;

    [Header("State")]
    public bool brainEnabled = true;
    public bool forceHoldPosition = false;

    [Header("Debug")]
    public bool drawDebug = true;
    public MoveMode currentMode = MoveMode.Orbit;

    private float thinkTimer;
    private float stuckTimer;
    private Vector3 lastPosition;
    private Vector3 currentTarget;
    private int orbitDirection = 1;

    private void Start()
    {
        if (movement == null)
            movement = GetComponent<BossMovement>();

        thinkTimer = Random.Range(0f, thinkInterval);
        lastPosition = transform.position;
        currentTarget = transform.position;

        orbitDirection = Random.value > 0.5f ? 1 : -1;
    }

    private void Update()
    {
        if (!brainEnabled || player == null || movement == null)
            return;

        if (forceHoldPosition)
        {
            currentMode = MoveMode.Hold;
            currentTarget = transform.position;
            movement.SetDesiredPosition(currentTarget);
            return;
        }

        thinkTimer -= Time.deltaTime;
        stuckTimer += Time.deltaTime;

        if (stuckTimer >= stuckCheckTime)
        {
            float movedDistance = Vector3.Distance(
                new Vector3(transform.position.x, 0f, transform.position.z),
                new Vector3(lastPosition.x, 0f, lastPosition.z)
            );

            if (movedDistance < 0.5f && !movement.HasReachedDesiredPosition())
            {
                ChooseRepositionTarget();
            }

            lastPosition = transform.position;
            stuckTimer = 0f;
        }

        if (thinkTimer <= 0f || movement.HasReachedDesiredPosition())
        {
            Think();
            thinkTimer = thinkInterval;
        }

        movement.SetDesiredPosition(currentTarget);
    }

    private void Think()
    {
        float distanceToPlayer = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(player.position.x, 0f, player.position.z)
        );

        if (IsOutsideArenaBounds())
        {
            ChooseRepositionTarget();
            return;
        }

        if (distanceToPlayer < tooCloseDistance)
        {
            ChooseRetreatTarget();
            return;
        }

        if (distanceToPlayer > tooFarDistance)
        {
            ChooseApproachToIdealRangeTarget();
            return;
        }

        if (distanceToPlayer >= idealMinDistance && distanceToPlayer <= idealMaxDistance)
        {
            if (Random.value < repositionChance)
            {
                ChooseRepositionTarget();
            }
            else
            {
                ChooseOrbitTarget();
            }

            return;
        }

        if (distanceToPlayer < idealMinDistance)
        {
            ChooseRetreatTarget();
            return;
        }

        if (distanceToPlayer > idealMaxDistance)
        {
            ChooseApproachToIdealRangeTarget();
            return;
        }

        ChooseOrbitTarget();
    }

    private void ChooseOrbitTarget()
    {
        currentMode = MoveMode.Orbit;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        toPlayer.Normalize();

        Vector3 sideways = Vector3.Cross(Vector3.up, toPlayer).normalized * orbitDirection;

        if (Random.value < 0.2f)
            orbitDirection *= -1;

        Vector3 candidate = transform.position + sideways * orbitStepDistance;

        float currentDistance = Vector3.Distance(
            new Vector3(candidate.x, 0f, candidate.z),
            new Vector3(player.position.x, 0f, player.position.z)
        );

        if (currentDistance < idealMinDistance)
        {
            Vector3 away = (candidate - player.position).normalized;
            away.y = 0f;
            candidate = player.position + away * idealMinDistance;
        }
        else if (currentDistance > idealMaxDistance)
        {
            Vector3 toward = (candidate - player.position).normalized;
            toward.y = 0f;
            candidate = player.position + toward * idealMaxDistance;
        }

        candidate.y = transform.position.y;

        if (!IsPositionValid(candidate))
        {
            ChooseRepositionTarget();
            return;
        }

        currentTarget = candidate;
    }

    private void ChooseRetreatTarget()
    {
        currentMode = MoveMode.Retreat;

        Vector3 away = transform.position - player.position;
        away.y = 0f;
        away.Normalize();

        Vector3 sideways = Vector3.Cross(Vector3.up, away).normalized;
        sideways *= (Random.value > 0.5f ? 1f : -1f);

        Vector3 candidate = transform.position + away * retreatDistance + sideways * (retreatDistance * 0.35f);
        candidate.y = transform.position.y;

        if (!IsPositionValid(candidate))
        {
            ChooseRepositionTarget();
            return;
        }

        currentTarget = candidate;
    }

    private void ChooseApproachToIdealRangeTarget()
    {
        currentMode = MoveMode.Orbit;

        Vector3 awayFromPlayer = transform.position - player.position;
        awayFromPlayer.y = 0f;
        awayFromPlayer.Normalize();

        float targetDistance = Random.Range(idealMinDistance, idealMaxDistance);
        Vector3 candidate = player.position + awayFromPlayer * targetDistance;
        candidate.y = transform.position.y;

        if (!IsPositionValid(candidate))
        {
            ChooseRepositionTarget();
            return;
        }

        currentTarget = candidate;
    }

    private void ChooseRepositionTarget()
    {
        currentMode = MoveMode.Reposition;

        for (int i = 0; i < 8; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            if (randomCircle == Vector2.zero)
                randomCircle = Vector2.right;

            float dist = Random.Range(idealMinDistance, repositionDistance);
            Vector3 candidate = player.position + new Vector3(randomCircle.x, 0f, randomCircle.y) * dist;
            candidate.y = transform.position.y;

            if (IsPositionValid(candidate))
            {
                currentTarget = candidate;
                return;
            }
        }

        currentTarget = ClampPointToBounds(transform.position);
    }

    private bool IsOutsideArenaBounds()
    {
        if (arenaBounds == null)
            return false;

        Bounds bounds = arenaBounds.bounds;
        Vector3 checkPos = new Vector3(transform.position.x, bounds.center.y, transform.position.z);
        return !bounds.Contains(checkPos);
    }

    private bool IsPositionValid(Vector3 candidate)
    {
        if (arenaBounds == null)
            return true;

        Bounds bounds = arenaBounds.bounds;
        Vector3 checkPos = new Vector3(candidate.x, bounds.center.y, candidate.z);
        return bounds.Contains(checkPos);
    }

    private Vector3 ClampPointToBounds(Vector3 point)
    {
        if (arenaBounds == null)
            return point;

        Bounds bounds = arenaBounds.bounds;

        float x = Mathf.Clamp(point.x, bounds.min.x, bounds.max.x);
        float z = Mathf.Clamp(point.z, bounds.min.z, bounds.max.z);

        return new Vector3(x, point.y, z);
    }

    public void SetBrainEnabled(bool enabled)
    {
        brainEnabled = enabled;
    }

    public void SetForceHoldPosition(bool hold)
    {
        forceHoldPosition = hold;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebug)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(currentTarget, 0.6f);

        if (arenaBounds != null)
        {
            Gizmos.color = Color.red;
            Bounds bounds = arenaBounds.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, currentTarget);
    }
}