using UnityEngine;

public class BossTurretTracker : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform turretBone;

    [Header("Tracking")]
    public float rotationSpeed = 6f;

    [Header("Offset")]
    public Vector3 rotationOffsetEuler;

    [Header("Clamp")]
    public bool useClamp = true;
    public float maxAngleFromForward = 80f;

    private void LateUpdate()
    {
        if (player == null || turretBone == null)
            return;

        Vector3 direction = player.position - turretBone.position;
        if (direction.sqrMagnitude < 0.001f)
            return;

        // Raw look rotation
        Quaternion lookRot = Quaternion.LookRotation(direction.normalized, Vector3.up);
        lookRot *= Quaternion.Euler(rotationOffsetEuler);

        if (useClamp)
        {
            // Clamp based on angle from forward
            float angle = Vector3.Angle(transform.forward, direction);

            if (angle > maxAngleFromForward)
            {
                Vector3 clampedDir = Vector3.RotateTowards(
                    transform.forward,
                    direction.normalized,
                    Mathf.Deg2Rad * maxAngleFromForward,
                    0f
                );

                lookRot = Quaternion.LookRotation(clampedDir, Vector3.up);
                lookRot *= Quaternion.Euler(rotationOffsetEuler);
            }
        }

        turretBone.rotation = Quaternion.Slerp(
            turretBone.rotation,
            lookRot,
            rotationSpeed * Time.deltaTime
        );
    }
}