using System.Collections;
using UnityEngine;

public class SniperEncounter : MonoBehaviour
{
    [Header("References")]
    public Transform sniperOrigin;
    public Transform player;
    public LineRenderer laserLine;

    [Header("Player Detection")]
    public string playerTag = "Player";
    public Vector3 playerAimOffset = new Vector3(0f, 1.2f, 0f);

    [Header("Shooting")]
    public float introShotDelay = 0.5f;
    public float timeBetweenShots = 2.5f;
    public int warningShotDamage = 20;
    public int normalShotDamage = 20;

    [Header("Raycast")]
    public float maxDistance = 100f;
    public LayerMask hitMask = ~0;

    [Header("Laser")]
    public bool showLaserOnlyWhenActive = true;

    [Header("State")]
    public bool encounterStarted = false;
    public bool introShotDone = false;
    public bool encounterDisabled = false;

    private Coroutine attackRoutine;
    private MonoBehaviour playerHealthScript;

    private void Start()
    {
        if (laserLine != null)
        {
            laserLine.enabled = !showLaserOnlyWhenActive;
            laserLine.positionCount = 2;
        }

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag(playerTag);
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }

        if (player != null)
        {
            playerHealthScript = player.GetComponent<MonoBehaviour>();
        }
    }

    private void Update()
    {
        if (!encounterStarted || encounterDisabled || player == null || sniperOrigin == null)
        {
            if (laserLine != null && showLaserOnlyWhenActive)
            {
                laserLine.enabled = false;
            }
            return;
        }

        UpdateLaser();
    }

    public void StartEncounter()
    {
        if (encounterStarted || encounterDisabled) return;

        encounterStarted = true;

        if (laserLine != null)
        {
            laserLine.enabled = true;
        }

        attackRoutine = StartCoroutine(AttackLoop());
    }

    public void DisableEncounter()
    {
        encounterDisabled = true;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        if (laserLine != null)
        {
            laserLine.enabled = false;
        }
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(introShotDelay);

        if (!introShotDone)
        {
            FireShot(warningShotDamage, true);
            introShotDone = true;
        }

        while (!encounterDisabled)
        {
            yield return new WaitForSeconds(timeBetweenShots);

            if (IsPlayerExposed())
            {
                FireShot(normalShotDamage, false);
            }
        }
    }

    private void UpdateLaser()
    {
        if (laserLine == null || player == null || sniperOrigin == null) return;

        Vector3 start = sniperOrigin.position;
        Vector3 target = player.position + playerAimOffset;
        Vector3 dir = (target - start).normalized;
        float distance = Vector3.Distance(start, target);

        Vector3 end = target;

        if (Physics.Raycast(
            start,
            dir,
            out RaycastHit hit,
            Mathf.Min(distance, maxDistance),
            hitMask,
            QueryTriggerInteraction.Ignore))
        {
            end = hit.point;
        }

        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);
    }

    private bool IsPlayerExposed()
    {
        if (player == null || sniperOrigin == null) return false;

        Vector3 start = sniperOrigin.position;
        Vector3 target = player.position + playerAimOffset;
        Vector3 dir = (target - start).normalized;
        float distance = Vector3.Distance(start, target);

        if (Physics.Raycast(
            start,
            dir,
            out RaycastHit hit,
            Mathf.Min(distance, maxDistance),
            hitMask,
            QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                return true;
            }

            return false;
        }

        return false;
    }

    private void FireShot(int damage, bool isWarningShot)
    {
        if (player == null || sniperOrigin == null) return;

        Vector3 start = sniperOrigin.position;
        Vector3 target = player.position + playerAimOffset;
        Vector3 dir = (target - start).normalized;
        float distance = Vector3.Distance(start, target);

        if (Physics.Raycast(
            start,
            dir,
            out RaycastHit hit,
            Mathf.Min(distance, maxDistance),
            hitMask,
            QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                ApplyDamageToPlayer(damage);

                if (isWarningShot)
                {
                    Debug.Log("Warning shot hit player.");
                }
                else
                {
                    Debug.Log("Sniper shot hit player.");
                }
            }
            else
            {
                Debug.Log("Shot blocked by cover: " + hit.transform.name);
            }
        }
    }

    private void ApplyDamageToPlayer(int damage)
    {
        if (player == null) return;

        var healthComponent = player.GetComponent("PlayerHealth");
        if (healthComponent != null)
        {
            var method = healthComponent.GetType().GetMethod("TakeDamage");
            if (method != null)
            {
                method.Invoke(healthComponent, new object[] { damage });
                return;
            }
        }

        healthComponent = player.GetComponentInChildren(System.Type.GetType("PlayerHealth"));
        if (healthComponent != null)
        {
            var method = healthComponent.GetType().GetMethod("TakeDamage");
            if (method != null)
            {
                method.Invoke(healthComponent, new object[] { damage });
                return;
            }
        }

        Debug.LogWarning("Could not find PlayerHealth.TakeDamage(int). Hook this up to your own health script.");
    }
}