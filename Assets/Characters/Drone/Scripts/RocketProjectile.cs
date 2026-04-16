using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 14f;
    public float lifeTime = 6f;

    [Header("Explosion")]
    public float explosionRadius = 2.5f;
    public int damage = 20;

    private Vector3 targetPosition;
    private Vector3 direction;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        direction = (targetPosition - transform.position).normalized;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        // TODO: add VFX later

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            // Example damage system hook
            var health = hit.GetComponentInParent<BossHealth>();
            if (health != null)
            {
                // ignore self if needed
            }

            // You’ll hook player damage here later
        }

        Destroy(gameObject);
    }
}