using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Ragdoll")]
    public float destroyAfterSeconds = 30f;
    public float hitForce = 6f;
    public bool disableCharacterControllerIfAny = true;

    [Tooltip("If your enemy has a root collider (capsule), it will be disabled on death.")]
    public bool disableRootColliderOnDeath = true;

    bool dead;

    Animator anim;
    NavMeshAgent agent;
    UtilityAIController ai;
    EnemyTracker tracker;

    Rigidbody[] ragdollBodies;
    Collider[] ragdollColliders;

    Collider rootCollider;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<UtilityAIController>();
        tracker = GetComponent<EnemyTracker>();

        ragdollBodies = GetComponentsInChildren<Rigidbody>(true);
        ragdollColliders = GetComponentsInChildren<Collider>(true);

        rootCollider = GetComponent<Collider>();

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Start animated, ragdoll OFF
        SetRagdoll(false);
    }

    // Allows your shotgun code to call IDamageable directly.
    public void TakeDamage(float amount)
    {
        ApplyDamage(amount, Vector3.zero, Vector3.zero);
    }

    // Use this when you want hit reaction force + proper ragdoll impulse.
    public void ApplyDamage(float amount, Vector3 hitPoint, Vector3 hitDir)
    {
        if (dead) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        if (currentHealth <= 0f)
            Die(hitPoint, hitDir);
    }

    // Handy for interact kills / debugging
    public void KillInstantly()
    {
        Vector3 hitPoint = transform.position + Vector3.up * 1.0f;
        Vector3 hitDir = -transform.forward;
        ApplyDamage(999999f, hitPoint, hitDir);
    }

    void Die(Vector3 hitPoint, Vector3 hitDir)
    {
        dead = true;

        // Stop AI + movement
        if (ai) ai.enabled = false;

        if (agent)
        {
            if (agent.isOnNavMesh)
                agent.isStopped = true;
            agent.enabled = false;
        }

        var behavior = GetComponent<EnemyBehavior>();
        if (behavior) behavior.enabled = false;

        if (anim) anim.enabled = false;

        if (disableCharacterControllerIfAny)
        {
            var cc = GetComponent<CharacterController>();
            if (cc) cc.enabled = false;
        }

        // Enable ragdoll
        SetRagdoll(true);

        if (disableRootColliderOnDeath && rootCollider)
            rootCollider.enabled = false;

        // Add hit force
        if (hitDir != Vector3.zero)
        {
            Rigidbody rb = FindClosestRigidbody(hitPoint);
            if (rb != null)
                rb.AddForce(hitDir.normalized * hitForce, ForceMode.Impulse);
        }

        // Notify wave system (once)
        if (tracker) tracker.ReportDeath();

        Destroy(gameObject, destroyAfterSeconds);
    }

    void SetRagdoll(bool enabled)
    {
        // When ragdoll is OFF: kinematic bodies, colliders OFF (so they don't mess with nav/aim)
        // When ragdoll is ON : non-kinematic, colliders ON
        for (int i = 0; i < ragdollBodies.Length; i++)
        {
            Rigidbody rb = ragdollBodies[i];
            if (!rb) continue;

            // Skip a potential rigidbody on the root object
            if (rb.gameObject == gameObject) continue;

            rb.isKinematic = !enabled;
            rb.detectCollisions = enabled;
            rb.interpolation = enabled ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }

        for (int i = 0; i < ragdollColliders.Length; i++)
        {
            Collider col = ragdollColliders[i];
            if (!col) continue;

            // Skip root collider here (handled separately)
            if (col.gameObject == gameObject) continue;

            col.enabled = enabled;
        }
    }

    Rigidbody FindClosestRigidbody(Vector3 hitPoint)
    {
        Rigidbody best = null;
        float bestDist = float.PositiveInfinity;

        for (int i = 0; i < ragdollBodies.Length; i++)
        {
            var rb = ragdollBodies[i];
            if (!rb || rb.gameObject == gameObject) continue;

            float d = (rb.worldCenterOfMass - hitPoint).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = rb;
            }
        }

        return best;
    }
}
