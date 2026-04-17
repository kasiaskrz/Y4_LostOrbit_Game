using UnityEngine;

public class BossHoverController : MonoBehaviour
{
    [Header("Main Hover Area")]
    public Transform hoverCenter;
    public float roamRadius = 6f;
    public float moveSpeed = 3f;
    public float arrivalDistance = 0.5f;
    public float repathDelay = 2f;

    [Header("Height / Floating")]
    public float hoverHeight = 4f;
    public float bobAmount = 0.35f;
    public float bobSpeed = 1.5f;

    [Header("Rotation")]
    public bool faceMovementDirection = true;
    public float rotationSpeed = 4f;
    public float yawOffset = 0f;

    [Header("Shield Node Override")]
    public bool allowShieldNodeMovement = true;
    public float shieldMoveSpeed = 4f;
    public float shieldArrivalDistance = 0.4f;

    [Header("Shield Freeze")]
    public bool freezeWhileShielded = true;
    public bool keepFacingWhileFrozen = false;
    public Transform frozenLookTarget;

    [Header("Debug")]
    public bool drawGizmos = true;

    private Vector3 currentTarget;
    private float repathTimer = 0f;

    private bool movingToShieldNode = false;
    private Transform currentShieldNode;

    private bool isShielded = false;
    private Vector3 frozenPosition;

    private void Start()
    {
        if (hoverCenter == null)
        {
            Debug.LogWarning($"{name}: No hoverCenter assigned. Using current position as hover center.");
            GameObject tempCenter = new GameObject(name + "_HoverCenter");
            tempCenter.transform.position = transform.position;
            hoverCenter = tempCenter.transform;
        }

        PickNewHoverTarget();
    }

    private void Update()
    {
        if (isShielded && freezeWhileShielded)
        {
            HandleFrozenShieldState();
            return;
        }

        if (movingToShieldNode && allowShieldNodeMovement && currentShieldNode != null)
        {
            HandleShieldNodeMovement();
        }
        else
        {
            HandleNormalHoverMovement();
        }
    }

    private void HandleFrozenShieldState()
    {
        transform.position = frozenPosition;

        if (keepFacingWhileFrozen && frozenLookTarget != null)
        {
            RotateTowards(frozenLookTarget.position);
        }
    }

    private void HandleNormalHoverMovement()
    {
        repathTimer -= Time.deltaTime;

        Vector3 targetPos = currentTarget;
        targetPos.y += Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        RotateTowards(targetPos);

        float flatDistance = Vector3.Distance(
            new Vector3(transform.position.x, hoverCenter.position.y, transform.position.z),
            new Vector3(currentTarget.x, hoverCenter.position.y, currentTarget.z)
        );

        if (flatDistance <= arrivalDistance || repathTimer <= 0f)
        {
            PickNewHoverTarget();
        }
    }

    private void HandleShieldNodeMovement()
    {
        Vector3 nodeTarget = currentShieldNode.position;
        nodeTarget.y += Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        transform.position = Vector3.MoveTowards(
            transform.position,
            nodeTarget,
            shieldMoveSpeed * Time.deltaTime
        );

        RotateTowards(nodeTarget);

        float dist = Vector3.Distance(transform.position, currentShieldNode.position);
        if (dist <= shieldArrivalDistance)
        {
            // Stay at node until released or frozen.
        }
    }

    private void RotateTowards(Vector3 targetPos)
    {
        if (!faceMovementDirection) return;

        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
        targetRot *= Quaternion.Euler(0f, yawOffset, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    private void PickNewHoverTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * roamRadius;

        currentTarget = new Vector3(
            hoverCenter.position.x + randomCircle.x,
            hoverCenter.position.y + hoverHeight,
            hoverCenter.position.z + randomCircle.y
        );

        repathTimer = repathDelay;
    }

    public void MoveToShieldNode(Transform shieldNode)
    {
        if (!allowShieldNodeMovement)
            return;

        if (shieldNode == null)
        {
            Debug.LogWarning($"{name}: MoveToShieldNode called with null node.");
            return;
        }

        currentShieldNode = shieldNode;
        movingToShieldNode = true;
    }

    public void StopShieldNodeMovement()
    {
        movingToShieldNode = false;
        currentShieldNode = null;
        PickNewHoverTarget();
    }

    public void SetShielded(bool shielded)
    {
        isShielded = shielded;

        if (isShielded)
        {
            frozenPosition = transform.position;
        }
        else
        {
            PickNewHoverTarget();
        }
    }

    public void FreezeNow()
    {
        isShielded = true;
        frozenPosition = transform.position;
    }

    public void Unfreeze()
    {
        isShielded = false;
        PickNewHoverTarget();
    }

    public void ForcePickNewHoverTarget()
    {
        PickNewHoverTarget();
    }

    public bool IsMovingToShieldNode()
    {
        return movingToShieldNode;
    }

    public bool IsShielded()
    {
        return isShielded;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Transform center = hoverCenter != null ? hoverCenter : transform;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center.position, roamRadius);

        Gizmos.color = Color.yellow;
        Vector3 hoverPos = new Vector3(center.position.x, center.position.y + hoverHeight, center.position.z);
        Gizmos.DrawWireSphere(hoverPos, 0.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(currentTarget, 0.2f);

        if (currentShieldNode != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentShieldNode.position);
            Gizmos.DrawSphere(currentShieldNode.position, 0.25f);
        }

        if (isShielded)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(frozenPosition, 0.35f);
        }
    }
}