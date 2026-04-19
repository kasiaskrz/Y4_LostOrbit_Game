using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Ragdoll")]
    public float hitForce = 6f;
    public bool disableCharacterControllerIfAny = true;

    [Tooltip("If your enemy has a root collider (capsule), it will be disabled on death.")]
    public bool disableRootColliderOnDeath = true;

    [Header("Explosion Death")]
    public bool explodeOnDeath = true;
    public float explodeDelay = 1f;
    public GameObject explosionVFXPrefab;
    public Transform explosionPoint;
    public Vector3 explosionOffset = Vector3.zero;
    public AudioClip explosionSound;
    [Range(0f, 1f)] public float explosionVolume = 1f;
    public bool destroyWholeRootObject = true;
    public float explosionScale = 3f;

    [Header("Drops")]
    public GameObject ammoPickupPrefab;
    public GameObject healthPickupPrefab;
    [Range(0f, 1f)] public float healthDropChance = 0.2f;
    public Vector3 dropOffset = new Vector3(0f, 0.2f, 0f);

    [Header("Drop Pop")]
    public float minDropPopForce = 1.2f;
    public float maxDropPopForce = 2.0f;
    public Vector2 dropSpreadRange = new Vector2(0.4f, 0.4f);

    [Header("Debug")]
    public bool debugDeathLogs = true;
    public KeyCode testExplodeKey = KeyCode.L;

    private bool dead;
    private bool deathSequenceStarted;
    public bool IsDead => dead;

    private Animator anim;
    private NavMeshAgent agent;
    private UtilityAIController ai;
    private EnemyTracker tracker;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    private Collider rootCollider;

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

        SetRagdoll(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(testExplodeKey))
        {
            Debug.Log($"{name}: Test explode key pressed.");
            StartCoroutine(ExplosionDeathRoutine());
        }
    }

    public void TakeDamage(float amount)
    {
        ApplyDamage(amount, Vector3.zero, Vector3.zero);
    }

    public void ApplyDamage(float amount, Vector3 hitPoint, Vector3 hitDir)
    {
        if (dead) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);

        if (currentHealth <= 0f)
            Die(hitPoint, hitDir);
    }

    public void KillInstantly()
    {
        Vector3 hitPoint = transform.position + Vector3.up * 1.0f;
        Vector3 hitDir = -transform.forward;
        ApplyDamage(999999f, hitPoint, hitDir);
    }

    void Die(Vector3 hitPoint, Vector3 hitDir)
    {
        if (dead) return;
        dead = true;

        if (debugDeathLogs)
            Debug.Log($"{name}: Die() called.");

        if (ai) ai.enabled = false;

        if (agent)
        {
            if (agent.isOnNavMesh)
                agent.isStopped = true;

            agent.enabled = false;
        }

        EnemyBehavior behavior = GetComponent<EnemyBehavior>();
        if (behavior) behavior.enabled = false;

        if (anim) anim.enabled = false;

        if (disableCharacterControllerIfAny)
        {
            CharacterController cc = GetComponent<CharacterController>();
            if (cc) cc.enabled = false;
        }

        SetRagdoll(true);

        if (disableRootColliderOnDeath && rootCollider)
            rootCollider.enabled = false;

        if (hitDir != Vector3.zero)
        {
            Rigidbody rb = FindClosestRigidbody(hitPoint);
            if (rb != null)
                rb.AddForce(hitDir.normalized * hitForce, ForceMode.Impulse);
        }

        if (tracker) tracker.ReportDeath();

        StartCoroutine(DropLootDelayed());

        if (explodeOnDeath)
        {
            if (debugDeathLogs)
                Debug.Log($"{name}: Starting explosion death routine.");

            StartCoroutine(ExplosionDeathRoutine());
        }
    }

    IEnumerator ExplosionDeathRoutine()
    {
        if (deathSequenceStarted) yield break;
        deathSequenceStarted = true;

        if (debugDeathLogs)
            Debug.Log($"{name}: Waiting {explodeDelay} seconds before explosion.");

        yield return new WaitForSeconds(explodeDelay);

        Vector3 spawnPos = transform.position + explosionOffset;

        if (explosionPoint != null)
            spawnPos = explosionPoint.position + explosionOffset;

        if (explosionVFXPrefab != null)
        {
            GameObject explosion = Instantiate(explosionVFXPrefab, spawnPos, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * explosionScale;

            if (debugDeathLogs)
                Debug.Log($"{name}: Explosion VFX spawned at {spawnPos} with scale {explosionScale}.");
        }
        else
        {
            Debug.LogWarning($"{name}: No explosionVFXPrefab assigned.");
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, spawnPos, explosionVolume);

            if (debugDeathLogs)
                Debug.Log($"{name}: Explosion sound played.");
        }

        if (destroyWholeRootObject)
        {
            if (debugDeathLogs)
                Debug.Log($"{name}: Destroying root object {transform.root.name}");

            Destroy(transform.root.gameObject);
        }
        else
        {
            if (debugDeathLogs)
                Debug.Log($"{name}: Destroying object {name}");

            Destroy(gameObject);
        }
    }

    IEnumerator DropLootDelayed()
    {
        yield return new WaitForSeconds(0.35f);

        Vector3 spawnCenter = transform.position + dropOffset;

        if (ammoPickupPrefab != null)
        {
            SpawnDrop(ammoPickupPrefab, spawnCenter);
        }

        if (healthPickupPrefab != null && Random.value <= healthDropChance)
        {
            SpawnDrop(healthPickupPrefab, spawnCenter);
        }
    }

    void SpawnDrop(GameObject prefab, Vector3 spawnCenter)
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-dropSpreadRange.x, dropSpreadRange.x),
            0f,
            Random.Range(-dropSpreadRange.y, dropSpreadRange.y)
        );

        Vector3 spawnPos = spawnCenter + randomOffset;
        spawnPos = GetGroundedSpawnPosition(prefab, spawnPos);

        Quaternion spawnRot = Quaternion.Euler(0f, prefab.transform.eulerAngles.y, 0f);
        GameObject drop = Instantiate(prefab, spawnPos, spawnRot);

        Rigidbody rb = drop.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            Vector3 popDir = new Vector3(
                Random.Range(-0.2f, 0.2f),
                Random.Range(0.9f, 1.1f),
                Random.Range(-0.2f, 0.2f)
            ).normalized;

            float popForce = Random.Range(minDropPopForce, maxDropPopForce);
            rb.AddForce(popDir * popForce, ForceMode.Impulse);
        }

        StartCoroutine(SettleDrop(drop));
    }

    IEnumerator SettleDrop(GameObject drop)
    {
        yield return new WaitForSeconds(0.4f);

        if (drop == null) yield break;

        Rigidbody rb = drop.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 euler = drop.transform.eulerAngles;
            drop.transform.rotation = Quaternion.Euler(0f, euler.y, 0f);
        }
    }

    Vector3 GetGroundedSpawnPosition(GameObject prefab, Vector3 testPos)
    {
        Vector3 rayStart = testPos + Vector3.up * 3f;

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 10f, ~0, QueryTriggerInteraction.Ignore))
        {
            float yOffset = 0.1f;

            Collider prefabCollider = prefab.GetComponentInChildren<Collider>();
            if (prefabCollider != null)
            {
                yOffset = prefabCollider.bounds.extents.y;
            }

            return new Vector3(testPos.x, hit.point.y + yOffset, testPos.z);
        }

        return testPos;
    }

    void SetRagdoll(bool enabled)
    {
        for (int i = 0; i < ragdollBodies.Length; i++)
        {
            Rigidbody rb = ragdollBodies[i];
            if (!rb) continue;

            if (rb.gameObject == gameObject) continue;

            rb.isKinematic = !enabled;
            rb.detectCollisions = enabled;
            rb.interpolation = enabled ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }

        for (int i = 0; i < ragdollColliders.Length; i++)
        {
            Collider col = ragdollColliders[i];
            if (!col) continue;

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
            Rigidbody rb = ragdollBodies[i];
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