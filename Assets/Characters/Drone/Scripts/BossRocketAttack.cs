using System.Collections;
using UnityEngine;

public class BossRocketAttack : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public GameObject rocketPrefab;
    public AudioSource audioSource;
    public AudioClip warningSound;
    public AudioClip launchSound;

    [Header("Timing")]
    public float minCooldown = 7f;
    public float maxCooldown = 8f;
    public float preFireDelay = 0.75f;

    [Header("Shield Behaviour")]
    public bool disableWhileShielded = true;
    public bool resetCooldownWhenShieldEnds = true;

    private float cooldownTimer;
    private bool isFiring = false;
    private bool isShielded = false;
    private Coroutine fireRoutineCoroutine;

    private void Start()
    {
        cooldownTimer = Random.Range(minCooldown, maxCooldown);
    }

    private void Update()
    {
        if (player == null || firePoint == null || rocketPrefab == null)
            return;

        if (disableWhileShielded && isShielded)
            return;

        if (isFiring)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            fireRoutineCoroutine = StartCoroutine(FireRoutine());
            cooldownTimer = Random.Range(minCooldown, maxCooldown);
        }
    }

    private IEnumerator FireRoutine()
    {
        isFiring = true;

        Vector3 lockedTargetPos = player.position - new Vector3(0f, 1.0f, 0f);

        if (audioSource != null && warningSound != null)
        {
            audioSource.PlayOneShot(warningSound);
        }

        yield return new WaitForSeconds(preFireDelay);

        if (disableWhileShielded && isShielded)
        {
            isFiring = false;
            fireRoutineCoroutine = null;
            yield break;
        }

        Vector3 spawnPos = firePoint.position + firePoint.forward * 0.75f;

        GameObject rocket = Instantiate(rocketPrefab, spawnPos, firePoint.rotation);

        RocketProjectile projectile = rocket.GetComponent<RocketProjectile>();
        if (projectile != null)
        {
            projectile.SetTarget(lockedTargetPos);

            Collider[] bossColliders = GetComponentsInChildren<Collider>();
            projectile.SetOwnerColliders(bossColliders);
        }

        if (audioSource != null && launchSound != null)
        {
            audioSource.PlayOneShot(launchSound);
        }

        isFiring = false;
        fireRoutineCoroutine = null;
    }

    public void SetShielded(bool shielded)
    {
        isShielded = shielded;

        if (isShielded)
        {
            if (fireRoutineCoroutine != null)
            {
                StopCoroutine(fireRoutineCoroutine);
                fireRoutineCoroutine = null;
            }

            isFiring = false;
        }
        else
        {
            if (resetCooldownWhenShieldEnds)
            {
                cooldownTimer = Random.Range(minCooldown, maxCooldown);
            }
        }
    }

    public void DisableRockets()
    {
        SetShielded(true);
    }

    public void EnableRockets()
    {
        SetShielded(false);
    }

    public bool IsShielded()
    {
        return isShielded;
    }
}