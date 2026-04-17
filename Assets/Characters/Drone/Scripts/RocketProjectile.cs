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

    [Header("Knockback")]
    public float maxKnockbackForce = 40f;
    public float minKnockbackForce = 18f;

    private Vector3 moveDirection;
    private bool hasTarget = false;
    private Collider[] ownerColliders;

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
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!hasTarget)
            return;

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore owner collision just in case
        if (IsOwnerCollider(collision.collider))
            return;

        Explode();
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hit = hits[i];

            // Skip owner completely
            if (IsOwnerCollider(hit))
                continue;

            float distance = Vector3.Distance(transform.position, hit.ClosestPoint(transform.position));
            float t = 1f - Mathf.Clamp01(distance / explosionRadius);

            int damage = Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, t));
            float knockbackForce = Mathf.Lerp(minKnockbackForce, maxKnockbackForce, t);

            PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            PlayerExplosionKnockback knockback = hit.GetComponentInParent<PlayerExplosionKnockback>();
            if (knockback != null)
            {
                Vector3 dir = hit.transform.position - transform.position;
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

            if (other.transform.IsChildOf(ownerColliders[i].transform) || ownerColliders[i].transform.IsChildOf(other.transform))
                return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}