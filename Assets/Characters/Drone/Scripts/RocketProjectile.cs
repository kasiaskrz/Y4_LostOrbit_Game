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
        Explode();
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, hits[i].ClosestPoint(transform.position));
            float t = 1f - Mathf.Clamp01(distance / explosionRadius);

            int damage = Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, t));
            float knockbackForce = Mathf.Lerp(minKnockbackForce, maxKnockbackForce, t);

            PlayerHealth playerHealth = hits[i].GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Rigidbody rb = hits[i].GetComponentInParent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                Vector3 dir = (hits[i].transform.position - transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }

            PlayerExplosionKnockback knockback = hits[i].GetComponentInParent<PlayerExplosionKnockback>();
            if (knockback != null)
            {
                Vector3 dir = hits[i].transform.position - transform.position;
                dir.y = 0f;

                if (dir.sqrMagnitude < 0.01f)
                {
                    dir = -transform.forward;
                }

                dir.Normalize();

                Vector3 finalForce = dir * knockbackForce;
                finalForce.y = knockbackForce * 0.6f;

                knockback.AddKnockback(finalForce);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}