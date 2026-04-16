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

    private float cooldownTimer;
    private bool isFiring = false;

    private void Start()
    {
        cooldownTimer = Random.Range(minCooldown, maxCooldown);
    }

    private void Update()
    {
        if (player == null || firePoint == null || rocketPrefab == null)
            return;

        if (isFiring)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            StartCoroutine(FireRoutine());
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
    }
}