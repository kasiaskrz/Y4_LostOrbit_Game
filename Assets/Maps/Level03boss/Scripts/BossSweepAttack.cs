using System.Collections;
using UnityEngine;

public class BossSweepAttack : MonoBehaviour
{
    [Header("References")]
    public Transform turretPivot;
    public Transform firePoint;
    public Transform sweepLeft;
    public Transform sweepRight;

    [Header("Scripts To Disable During Attack")]
    public MonoBehaviour normalTurretTracking;
    public MonoBehaviour bossMovementScript;

    [Header("Sweep Settings")]
    public float preAimRotateSpeed = 180f;
    public float sweepDuration = 2.5f;
    public float fireInterval = 0.03f;
    public float attackCooldown = 4f;

    [Header("Rotation Offset")]
    public float yawOffset = 90f;

    [Header("Shooting")]
    public float shotRange = 100f;
    public int shotDamage = 5;
    public LayerMask hitLayers = ~0;

    [Header("Tracer")]
    public ShotTracer tracerPrefab;

    [Header("Testing")]
    public KeyCode testKey = KeyCode.T;

    private bool isAttacking = false;
    private bool canAttack = true;

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            TryStartSweepAttack();
        }
    }

    public void TryStartSweepAttack()
    {
        if (!canAttack || isAttacking) return;
        if (turretPivot == null || firePoint == null || sweepLeft == null || sweepRight == null) return;

        StartCoroutine(SweepAttackRoutine());
    }

    private IEnumerator SweepAttackRoutine()
    {
        isAttacking = true;
        canAttack = false;

        if (normalTurretTracking != null)
            normalTurretTracking.enabled = false;

        if (bossMovementScript != null)
            bossMovementScript.enabled = false;

        // Rotate to the sweep start first
        yield return StartCoroutine(RotateToPoint(sweepLeft.position));

        Quaternion startRot = GetFlatLookRotation(sweepLeft.position);
        Quaternion endRot = GetFlatLookRotation(sweepRight.position);

        float elapsed = 0f;
        float fireTimer = 0f;

        while (elapsed < sweepDuration)
        {
            elapsed += Time.deltaTime;
            fireTimer += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / sweepDuration);
            turretPivot.rotation = Quaternion.Slerp(startRot, endRot, t);

            while (fireTimer >= fireInterval)
            {
                fireTimer -= fireInterval;
                FireShot();
            }

            yield return null;
        }

        turretPivot.rotation = endRot;

        if (normalTurretTracking != null)
            normalTurretTracking.enabled = true;

        if (bossMovementScript != null)
            bossMovementScript.enabled = true;

        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator RotateToPoint(Vector3 point)
    {
        Quaternion targetRot = GetFlatLookRotation(point);

        while (Quaternion.Angle(turretPivot.rotation, targetRot) > 1f)
        {
            turretPivot.rotation = Quaternion.RotateTowards(
                turretPivot.rotation,
                targetRot,
                preAimRotateSpeed * Time.deltaTime
            );

            yield return null;
        }

        turretPivot.rotation = targetRot;
    }

    private Quaternion GetFlatLookRotation(Vector3 point)
    {
        Vector3 dir = point - turretPivot.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            dir = turretPivot.forward;

        Quaternion baseRotation = Quaternion.LookRotation(dir);
        return baseRotation * Quaternion.Euler(0f, yawOffset, 0f);
    }

    private void FireShot()
    {
        if (firePoint == null) return;

        Vector3 startPoint = firePoint.position;
        Vector3 direction = firePoint.forward;

        Ray ray = new Ray(startPoint, direction);
        RaycastHit hit;
        Vector3 endPoint;

        if (Physics.Raycast(ray, out hit, shotRange, hitLayers))
        {
            endPoint = hit.point;

            // Optional damage hook
            // PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();
            // if (health != null)
            // {
            //     health.TakeDamage(shotDamage);
            // }
        }
        else
        {
            endPoint = startPoint + direction * shotRange;
        }

        SpawnTracer(startPoint, endPoint);
    }

    private void SpawnTracer(Vector3 startPoint, Vector3 endPoint)
    {
        if (tracerPrefab == null) return;

        ShotTracer tracer = Instantiate(tracerPrefab, startPoint, Quaternion.identity);
        tracer.Init(startPoint, endPoint);
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}