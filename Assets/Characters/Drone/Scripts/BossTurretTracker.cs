using UnityEngine;

public class BossTurretTracker : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform turretBone;

    [Header("Tracking")]
    public float rotationSpeed = 6f;
    public bool trackPlayer = true;

    [Header("Fixed Offset")]
    public Vector3 rotationOffsetEuler = new Vector3(0f, -90f, 0f);

    private void LateUpdate()
    {
        if (!trackPlayer || player == null || turretBone == null)
            return;

        Vector3 direction = player.position - turretBone.position;

        if (direction.sqrMagnitude <= 0.0001f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        lookRotation *= Quaternion.Euler(rotationOffsetEuler);

        turretBone.rotation = Quaternion.Slerp(
            turretBone.rotation,
            lookRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}