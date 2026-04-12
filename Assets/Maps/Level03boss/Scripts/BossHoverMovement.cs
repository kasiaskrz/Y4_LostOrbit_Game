using UnityEngine;

public class BossHoverMovement : MonoBehaviour
{
    [Header("Movement Area")]
    public Transform centerPoint;
    public float moveRadius = 4f;
    public float moveSpeed = 2f;
    public float reachDistance = 0.3f;

    [Header("Hover")]
    public float hoverHeight = 0.25f;
    public float hoverSpeed = 1.5f;

    [Header("Wobble")]
    public float wobbleAmount = 4f;
    public float wobbleSpeed = 2f;

    [Header("Optional Tilt While Moving")]
    public bool tiltTowardsMovement = true;
    public float maxTilt = 10f;
    public float tiltSmoothSpeed = 4f;

    private Vector3 currentTarget;
    private Vector3 basePosition;
    private Vector3 lastPosition;
    private Quaternion baseRotation;
    private bool hasTarget = false;

    private void Start()
    {
        baseRotation = transform.rotation;
        lastPosition = transform.position;

        if (centerPoint == null)
        {
            Debug.LogWarning("BossHoverMovement: No centerPoint assigned. Using current position.");
            GameObject tempCenter = new GameObject(name + "_CenterPoint");
            tempCenter.transform.position = transform.position;
            centerPoint = tempCenter.transform;
        }

        PickNewTarget();
    }

    private void Update()
    {
        HandleMovement();
        HandleHoverAndWobble();
        lastPosition = transform.position;
    }

    private void HandleMovement()
    {
        if (!hasTarget)
        {
            PickNewTarget();
        }

        Vector3 moveTarget = new Vector3(currentTarget.x, centerPoint.position.y, currentTarget.z);

        Vector3 newPos = Vector3.MoveTowards(
            new Vector3(transform.position.x, centerPoint.position.y, transform.position.z),
            moveTarget,
            moveSpeed * Time.deltaTime
        );

        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);

        float dist = Vector3.Distance(
            new Vector3(transform.position.x, centerPoint.position.y, transform.position.z),
            moveTarget
        );

        if (dist <= reachDistance)
        {
            PickNewTarget();
        }
    }

    private void HandleHoverAndWobble()
    {
        float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        float wobbleZ = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        float wobbleX = Mathf.Cos(Time.time * wobbleSpeed * 0.8f) * wobbleAmount * 0.5f;

        float targetY = centerPoint.position.y + hoverOffset;
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);

        Quaternion wobbleRotation = Quaternion.Euler(wobbleX, baseRotation.eulerAngles.y, wobbleZ);

        if (tiltTowardsMovement)
        {
            Vector3 velocity = (transform.position - lastPosition) / Mathf.Max(Time.deltaTime, 0.0001f);
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float pitch = Mathf.Clamp(-localVelocity.z, -1f, 1f) * maxTilt;
            float roll = Mathf.Clamp(localVelocity.x, -1f, 1f) * -maxTilt;

            Quaternion movementTilt = Quaternion.Euler(
                wobbleX + pitch,
                baseRotation.eulerAngles.y,
                wobbleZ + roll
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                movementTilt,
                tiltSmoothSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                wobbleRotation,
                tiltSmoothSpeed * Time.deltaTime
            );
        }
    }

    private void PickNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * moveRadius;
        currentTarget = new Vector3(
            centerPoint.position.x + randomCircle.x,
            centerPoint.position.y,
            centerPoint.position.z + randomCircle.y
        );

        hasTarget = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (centerPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(centerPoint.position, moveRadius);
        }
    }
}