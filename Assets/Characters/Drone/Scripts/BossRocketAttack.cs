using System.Collections;
using UnityEngine;

public class BossRocketAttack : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public GameObject rocketPrefab;
    public AudioSource audioSource;

    [Header("Timing")]
    public float minCooldown = 7f;
    public float maxCooldown = 8f;
    public float preFireDelay = 0.75f;

    [Header("Audio")]
    public AudioClip warningSound;

    [Header("State")]
    public bool canAttack = true;

    private float cooldownTimer;
    private bool isFiring = false;

    private void Start()
    {
        cooldownTimer = Random.Range(minCooldown, maxCooldown);
    }

    private void Update()
    {
        if (!canAttack || player == null || firePoint == null || rocketPrefab == null)
            return;

        if (isFiring)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            StartCoroutine(FireRocketRoutine());
            cooldownTimer = Random.Range(minCooldown, maxCooldown);
        }
    }

    private IEnumerator FireRocketRoutine()
    {
        isFiring = true;

        // Store player position (VERY IMPORTANT for dodge mechanic)
        Vector3 targetPosition = player.position;

        // Play warning sound
        if (audioSource != null && warningSound != null)
        {
            audioSource.PlayOneShot(warningSound);
        }

        // Wait before firing (gives player time to react)
        yield return new WaitForSeconds(preFireDelay);

        // Spawn rocket
        GameObject rocket = Instantiate(
            rocketPrefab,
            firePoint.position,
            firePoint.rotation
        );

        // Pass target position to rocket
        RocketProjectile proj = rocket.GetComponent<RocketProjectile>();
        if (proj != null)
        {
            proj.SetTarget(targetPosition);
        }

        isFiring = false;
    }

    public void SetCanAttack(bool value)
    {
        canAttack = value;
    }
}