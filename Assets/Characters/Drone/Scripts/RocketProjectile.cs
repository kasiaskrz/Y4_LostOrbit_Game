using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 9f;
    public float lifetime = 6f;

    [Header("Explosion")]
    public float explosionRadius = 5f;
    public int maxDamage = 35;
    public int minDamage = 10;
    public GameObject explosionVFXPrefab;
    public LayerMask groundLayers = ~0;

    [Header("Knockback")]
    public float maxKnockbackForce = 40f;
    public float minKnockbackForce = 18f;

    [Header("Audio")]
    public AudioSource rocketTravelAudioSource;
    public AudioClip explosionSound;
    [Range(0f, 2f)] public float explosionVolume = 1f;

    [Header("Debug")]
    public bool debugExplosion = true;
    public bool debugDrawExplosionRadius = true;
    public float debugDrawTime = 2f;

    private Vector3 moveDirection;
    private bool hasTarget = false;
    private Collider[] ownerColliders;
    private bool hasExploded = false;

    public void SetTarget(Vector3 targetPosition)
    {
        moveDirection = (targetPosition - transform.position).normalized;
        hasTarget = true;
    }

    public void SetOwnerColliders(Collider[] collidersToIgnore)
    {
        ownerColliders = collidersToIgnore;

        Collider myCollider = GetComponent<Collider>();
        if (myCollider == null || ownerColliders == null)
            return;

        for (int i = 0; i < ownerColliders.Length; i++)
        {
            if (ownerColliders[i] != null)
            {
                Physics.IgnoreCollision(myCollider, ownerColliders[i], true);
            }
        }
    }

    private void Start()
    {
        if (rocketTravelAudioSource != null)
        {
            rocketTravelAudioSource.loop = true;

            if (!rocketTravelAudioSource.isPlaying)
            {
                rocketTravelAudioSource.Play();
            }
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!hasTarget || hasExploded)
            return;

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded)
            return;

        // Ignore owner collision just in case
        if (IsOwnerCollider(collision.collider))
            return;

        Vector3 hitPoint = collision.contacts.Length > 0
            ? collision.contacts[0].point
            : transform.position;

        if (debugExplosion)
        {
            Debug.Log(
                "[RocketProjectile] Hit: " + collision.collider.name +
                " | Layer: " + LayerMask.LayerToName(collision.collider.gameObject.layer) +
                " | Hit Point: " + hitPoint +
                " | Explosion Radius: " + explosionRadius,
                this
            );
        }

        Explode(hitPoint);
    }

    private void Explode(Vector3 explosionPoint)
    {
        if (hasExploded)
            return;

        hasExploded = true;

        // Stop rocket travel sound
        if (rocketTravelAudioSource != null && rocketTravelAudioSource.isPlaying)
        {
            rocketTravelAudioSource.Stop();
        }

        // Spawn explosion VFX
        if (explosionVFXPrefab != null)
        {
            GameObject explosion = Instantiate(explosionVFXPrefab, explosionPoint, Quaternion.identity);
            Destroy(explosion, 4f);
        }
        else if (debugExplosion)
        {
            Debug.LogWarning("[RocketProjectile] No explosionVFXPrefab assigned.", this);
        }

        // Play explosion sound
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, explosionPoint, explosionVolume);
        }
        else if (debugExplosion)
        {
            Debug.LogWarning("[RocketProjectile] No explosionSound assigned.", this);
        }

        // Draw debug explosion radius in scene view
        if (debugDrawExplosionRadius)
        {
            DrawDebugSphere(explosionPoint, explosionRadius, debugDrawTime, 20);
        }

        Collider[] hits = Physics.OverlapSphere(explosionPoint, explosionRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hit = hits[i];

            // Skip owner completely
            if (IsOwnerCollider(hit))
                continue;

            float distance = Vector3.Distance(explosionPoint, hit.ClosestPoint(explosionPoint));
            float t = 1f - Mathf.Clamp01(distance / explosionRadius);

            int damage = Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, t));
            float knockbackForce = Mathf.Lerp(minKnockbackForce, maxKnockbackForce, t);

            if (debugExplosion)
            {
                Debug.Log(
                    "[RocketProjectile] Affected: " + hit.name +
                    " | Distance: " + distance.ToString("F2") +
                    " | Damage: " + damage +
                    " | Knockback: " + knockbackForce.ToString("F2"),
                    hit
                );
            }

            PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            PlayerExplosionKnockback knockback = hit.GetComponentInParent<PlayerExplosionKnockback>();
            if (knockback != null)
            {
                Vector3 dir = hit.transform.position - explosionPoint;
                dir.y = 0f;

                if (dir.sqrMagnitude < 0.01f)
                    dir = -transform.forward;

                dir.Normalize();

                Vector3 finalForce = dir * knockbackForce;
                finalForce.y = knockbackForce * 0.6f;

                knockback.AddKnockback(finalForce);
            }
        }

        Destroy(gameObject);
    }

    private bool IsOwnerCollider(Collider other)
    {
        if (ownerColliders == null || other == null)
            return false;

        for (int i = 0; i < ownerColliders.Length; i++)
        {
            if (ownerColliders[i] == null)
                continue;

            if (other == ownerColliders[i])
                return true;

            if (other.transform.IsChildOf(ownerColliders[i].transform) ||
                ownerColliders[i].transform.IsChildOf(other.transform))
                return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void DrawDebugSphere(Vector3 center, float radius, float duration, int segments)
    {
        float step = 360f / segments;

        // XZ ring
        for (int i = 0; i < segments; i++)
        {
            float a1 = Mathf.Deg2Rad * (i * step);
            float a2 = Mathf.Deg2Rad * ((i + 1) * step);

            Vector3 p1 = center + new Vector3(Mathf.Cos(a1) * radius, 0f, Mathf.Sin(a1) * radius);
            Vector3 p2 = center + new Vector3(Mathf.Cos(a2) * radius, 0f, Mathf.Sin(a2) * radius);
            Debug.DrawLine(p1, p2, Color.red, duration);
        }

        // XY ring
        for (int i = 0; i < segments; i++)
        {
            float a1 = Mathf.Deg2Rad * (i * step);
            float a2 = Mathf.Deg2Rad * ((i + 1) * step);

            Vector3 p1 = center + new Vector3(Mathf.Cos(a1) * radius, Mathf.Sin(a1) * radius, 0f);
            Vector3 p2 = center + new Vector3(Mathf.Cos(a2) * radius, Mathf.Sin(a2) * radius, 0f);
            Debug.DrawLine(p1, p2, Color.yellow, duration);
        }

        // YZ ring
        for (int i = 0; i < segments; i++)
        {
            float a1 = Mathf.Deg2Rad * (i * step);
            float a2 = Mathf.Deg2Rad * ((i + 1) * step);

            Vector3 p1 = center + new Vector3(0f, Mathf.Cos(a1) * radius, Mathf.Sin(a1) * radius);
            Vector3 p2 = center + new Vector3(0f, Mathf.Cos(a2) * radius, Mathf.Sin(a2) * radius);
            Debug.DrawLine(p1, p2, Color.cyan, duration);
        }
    }
}