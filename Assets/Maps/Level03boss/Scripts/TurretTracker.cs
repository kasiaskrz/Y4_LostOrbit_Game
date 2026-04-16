using UnityEngine;

public class BossTurretTracker : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Rotation")]
    public float rotationSpeed = 5f;

    [Header("Axis Control")]
    public bool rotateX = false;
    public bool rotateY = true;
    public bool rotateZ = false;

    [Header("Forward Axis Fix")]
    public Vector3 modelForwardOffset;

    [Header("Optional Limits")]
    public bool clampX = false;
    public float minX = -30f;
    public float maxX = 30f;

    public bool clampY = false;
    public float minY = -90f;
    public float maxY = 90f;

    public bool clampZ = false;
    public float minZ = -20f;
    public float maxZ = 20f;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;

        if (direction.sqrMagnitude <= 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion correctedRotation = lookRotation * Quaternion.Euler(modelForwardOffset);

        Vector3 euler = correctedRotation.eulerAngles;
        Vector3 currentEuler = transform.rotation.eulerAngles;

        float x = currentEuler.x;
        float y = currentEuler.y;
        float z = currentEuler.z;

        if (rotateX) x = NormalizeAngle(euler.x);
        if (rotateY) y = NormalizeAngle(euler.y);
        if (rotateZ) z = NormalizeAngle(euler.z);

        if (clampX) x = Mathf.Clamp(x, minX, maxX);
        if (clampY) y = Mathf.Clamp(y, minY, maxY);
        if (clampZ) z = Mathf.Clamp(z, minZ, maxZ);

        Quaternion targetRotation = Quaternion.Euler(x, y, z);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}