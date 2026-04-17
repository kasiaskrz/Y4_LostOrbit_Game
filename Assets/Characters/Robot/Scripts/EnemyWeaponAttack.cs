using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWeaponAttack : MonoBehaviour
{
    public enum EnemyAttackType
    {
        Shotgun,
        Rocket
    }

    [Header("Type")]
    public EnemyAttackType attackType = EnemyAttackType.Shotgun;

    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public NavMeshAgent agent;
    public Animator animator;
    public EnemyHealth enemyHealth;

    [Header("Shotgun")]
    public ShotgunShooter shotgunShooter;
    public float shotgunRange = 15f;
    public float shotgunCooldown = 2f;

    [Header("Rocket")]
    public GameObject rocketPrefab;
    public float rocketRange = 20f;
    public float rocketCooldown = 6f;
    public float rocketPreFireDelay = 0.6f;
    public float rocketTargetHeightOffset = -1.2f;

    [Header("Rotation")]
    public float faceSpeed = 6f;

    [Header("State")]
    public bool canAttack = true;

    private float attackTimer;
    private bool isAttacking = false;

    private void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        if (enemyHealth == null)
            enemyHealth = GetComponent<EnemyHealth>();

        if (agent != null)
            agent.updateRotation = true;
    }

    private void Update()
    {
        if (!canAttack || player == null || firePoint == null)
            return;

        if (enemyHealth != null && enemyHealth.IsDead)
            return;

        attackTimer -= Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);

        if (isAttacking)
        {
            FacePlayer();
            return;
        }

        if (attackType == EnemyAttackType.Shotgun)
        {
            if (distance <= shotgunRange && attackTimer <= 0f)
            {
                StartCoroutine(FireShotgunRoutine());
                attackTimer = shotgunCooldown;
            }
        }
        else if (attackType == EnemyAttackType.Rocket)
        {
            if (distance <= rocketRange && attackTimer <= 0f && rocketPrefab != null)
            {
                StartCoroutine(FireRocketRoutine());
                attackTimer = rocketCooldown;
            }
        }
    }

    private IEnumerator FireShotgunRoutine()
    {
        isAttacking = true;

        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;

        yield return new WaitForSeconds(0.15f);

        if (enemyHealth != null && enemyHealth.IsDead)
        {
            if (agent != null && agent.isActiveAndEnabled)
                agent.isStopped = false;

            isAttacking = false;
            yield break;
        }

        FacePlayer();

        if (shotgunShooter != null)
            shotgunShooter.FireOnce();

        yield return new WaitForSeconds(0.35f);

        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = false;

        isAttacking = false;
    }

    private IEnumerator FireRocketRoutine()
    {
        isAttacking = true;

        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = true;

        Vector3 lockedTarget = player.position + new Vector3(0f, rocketTargetHeightOffset, 0f);

        float timer = 0f;
        while (timer < rocketPreFireDelay)
        {
            if (enemyHealth != null && enemyHealth.IsDead)
            {
                if (agent != null && agent.isActiveAndEnabled)
                    agent.isStopped = false;

                isAttacking = false;
                yield break;
            }

            FacePlayer();
            timer += Time.deltaTime;
            yield return null;
        }

        GameObject rocket = Instantiate(
            rocketPrefab,
            firePoint.position + firePoint.forward * 0.75f,
            firePoint.rotation
        );

        RocketProjectile projectile = rocket.GetComponent<RocketProjectile>();
        if (projectile != null)
        {
            projectile.SetTarget(lockedTarget);

            Collider[] myColliders = GetComponentsInChildren<Collider>();
            projectile.SetOwnerColliders(myColliders);
        }

        yield return new WaitForSeconds(0.4f);

        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = false;

        isAttacking = false;
    }

    private void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0f;

        if (lookPos.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(lookPos.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, faceSpeed * Time.deltaTime);
    }
}