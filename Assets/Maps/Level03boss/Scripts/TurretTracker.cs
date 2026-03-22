using UnityEngine;

public class TurretTracker : MonoBehaviour
{
    [Header("References")]
    public Transform turret;   // your turret mesh
    public Transform player;   // player transform

    [Header("Settings")]
    public float rotationSpeed = 5f;
    public bool lockY = true; // keeps turret flat (no up/down tilt)

    void Update()
    {
        if (turret == null || player == null) return;

        Vector3 direction = player.position - turret.position;

        if (lockY)
            direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        turret.rotation = Quaternion.Slerp(
            turret.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }
}