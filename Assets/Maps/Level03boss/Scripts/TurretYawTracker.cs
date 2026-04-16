using UnityEngine;

public class TurretYawTracker : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Settings")]
    public float turnSpeed = 5f;
    public float yawOffset = 0f;

    [Header("Optional")]
    public bool findPlayerByTag = true;
    public string playerTag = "Player";

    private void Start()
    {
        if (target == null && findPlayerByTag)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
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

        // Ignore height so the turret only rotates left/right
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f) return;

        float targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, targetYaw + yawOffset, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }
}