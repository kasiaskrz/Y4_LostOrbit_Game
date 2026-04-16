using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float acceleration = 4f;
    public float arriveDistance = 1.25f;

    [Header("Hover")]
    public float hoverAmplitude = 0.35f;
    public float hoverSpeed = 1.8f;

    [Header("Rotation")]
    public float rotationSpeed = 4f;
    public float tiltAmount = 10f;
    public float tiltSmoothSpeed = 4f;

    [Header("Debug")]
    public bool drawDebug = true;

    private Vector3 desiredPosition;
    private Vector3 velocity;
    private Vector3 basePosition;
    private bool hasDesiredPosition = false;
    private bool movementEnabled = true;

    private float hoverOffset;
    private Quaternion visualTilt = Quaternion.identity;

    public Vector3 CurrentDesiredPosition => desiredPosition;
    public bool HasDesiredPosition => hasDesiredPosition;
    public bool MovementEnabled => movementEnabled;

    private void Start()
    {
        basePosition = transform.position;
        hoverOffset = Random.Range(0f, 100f);
        desiredPosition = transform.position;
    }

    private void Update()
    {
        if (!movementEnabled)
        {
            ApplyHoverOnly();
            return;
        }

        MoveTowardsDesiredPosition();
    }

    public void SetDesiredPosition(Vector3 worldPosition)
    {
        desiredPosition = worldPosition;
        hasDesiredPosition = true;
    }

    public void ClearDesiredPosition()
    {
        desiredPosition = transform.position;
        hasDesiredPosition = false;
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (!enabled)
        {
            velocity = Vector3.zero;
        }
    }

    public bool HasReachedDesiredPosition()
    {
        Vector3 flatCurrent = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatTarget = new Vector3(desiredPosition.x, 0f, desiredPosition.z);
        return Vector3.Distance(flatCurrent, flatTarget) <= arriveDistance;
    }

    private void MoveTowardsDesiredPosition()
    {
        Vector3 currentPosition = transform.position;

        Vector3 flatCurrent = new Vector3(currentPosition.x, 0f, currentPosition.z);
        Vector3 flatTarget = new Vector3(desiredPosition.x, 0f, desiredPosition.z);

        Vector3 toTarget = flatTarget - flatCurrent;
        float distance = toTarget.magnitude;

        Vector3 moveDirection = distance > 0.001f ? toTarget.normalized : Vector3.zero;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        velocity = Vector3.Lerp(velocity, targetVelocity, acceleration * Time.deltaTime);

        if (distance <= arriveDistance)
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, acceleration * Time.deltaTime);
        }

        Vector3 horizontalMove = velocity * Time.deltaTime;

        float hoverY = Mathf.Sin((Time.time + hoverOffset) * hoverSpeed) * hoverAmplitude;
        Vector3 nextPosition = currentPosition + new Vector3(horizontalMove.x, 0f, horizontalMove.z);

        nextPosition.y = desiredPosition.y + hoverY;
        transform.position = nextPosition;

        ApplyRotationAndTilt(moveDirection, horizontalMove);
    }

    private void ApplyHoverOnly()
    {
        Vector3 currentPosition = transform.position;
        float hoverY = Mathf.Sin((Time.time + hoverOffset) * hoverSpeed) * hoverAmplitude;

        Vector3 nextPosition = currentPosition;
        nextPosition.y = basePosition.y + hoverY;
        transform.position = nextPosition;

        Quaternion targetTilt = Quaternion.identity;
        visualTilt = Quaternion.Slerp(visualTilt, targetTilt, tiltSmoothSpeed * Time.deltaTime);
    }

    private void ApplyRotationAndTilt(Vector3 moveDirection, Vector3 horizontalMove)
    {
        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            Vector3 localMove = transform.InverseTransformDirection(horizontalMove.normalized);
            float roll = -localMove.x * tiltAmount;
            float pitch = localMove.z * tiltAmount * 0.35f;

            Quaternion tiltRotation = Quaternion.Euler(pitch, 0f, roll);
            visualTilt = Quaternion.Slerp(visualTilt, tiltRotation, tiltSmoothSpeed * Time.deltaTime);

            Quaternion finalRotation = lookRotation * visualTilt;
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            visualTilt = Quaternion.Slerp(visualTilt, Quaternion.identity, tiltSmoothSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * visualTilt, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebug)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(desiredPosition, 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, desiredPosition);
    }
}