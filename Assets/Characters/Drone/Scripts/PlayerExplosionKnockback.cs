using UnityEngine;

public class PlayerExplosionKnockback : MonoBehaviour
{
    [Header("Knockback")]
    public float horizontalDamp = 2.5f;
    public float verticalDamp = 1.8f;
    public float stopThreshold = 0.15f;

    private Vector3 knockbackVelocity;

    public Vector3 CurrentKnockback => knockbackVelocity;
    public bool IsBeingKnockedBack => knockbackVelocity.sqrMagnitude > 0.01f;

    public void AddKnockback(Vector3 force)
    {
        knockbackVelocity += force;
    }

    public void ClearKnockback()
    {
        knockbackVelocity = Vector3.zero;
    }

    public void TickDamping(float deltaTime)
    {
        knockbackVelocity.x = Mathf.Lerp(knockbackVelocity.x, 0f, horizontalDamp * deltaTime);
        knockbackVelocity.z = Mathf.Lerp(knockbackVelocity.z, 0f, horizontalDamp * deltaTime);
        knockbackVelocity.y = Mathf.Lerp(knockbackVelocity.y, 0f, verticalDamp * deltaTime);

        if (knockbackVelocity.magnitude < stopThreshold)
        {
            knockbackVelocity = Vector3.zero;
        }
    }
}