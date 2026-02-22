using UnityEngine;
using UnityEngine.AI;

public class EnemyBlackboard : MonoBehaviour
{
    [Header("Refs")]
    public NavMeshAgent agent;
    public Animator animator;
    public EnemySensors sensors;
    public Transform muzzle;

    [Header("Combat")]
    public float preferredRange = 12f;      // shotgun sweet spot
    public float tooCloseRange = 4f;        // back off / reposition
    public float fireCooldown = 0.9f;
    public int pellets = 10;
    public float spreadDegrees = 4.5f;
    public float range = 25f;
    public float damagePerPellet = 6f;
    public LayerMask hitMask = ~0;

    [Header("Cover")]
    public float coverSearchRadius = 18f;
    public float coverRepathCooldown = 0.5f;

    [HideInInspector] public CoverPoint currentCover;
    [HideInInspector] public float nextFireTime;
    [HideInInspector] public float nextRepathTime;

    public bool InCover => currentCover != null;

    void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        sensors = GetComponent<EnemySensors>();
    }
}
