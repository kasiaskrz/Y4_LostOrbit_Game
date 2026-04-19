using System.Collections;
using UnityEngine;

public class RobotDeathExploder : MonoBehaviour
{
    [Header("Explosion Timing")]
    public float explodeDelay = 1f;

    [Header("Explosion VFX")]
    public GameObject explosionVFXPrefab;
    public Transform explosionPoint;
    public Vector3 explosionOffset = Vector3.zero;

    [Header("Explosion Audio")]
    public AudioClip explosionSound;
    [Range(0f, 1f)] public float explosionVolume = 1f;

    [Header("Destroy")]
    public bool destroyWholeRootObject = true;

    [Header("Debug")]
    public bool debugLog = true;
    public KeyCode testExplodeKey = KeyCode.L;

    private bool hasExploded = false;

    private void Update()
    {
        if (Input.GetKeyDown(testExplodeKey))
        {
            if (debugLog) Debug.Log($"{name}: Test explode key pressed.");
            TriggerExplosionDeath();
        }
    }

    public void TriggerExplosionDeath()
    {
        if (hasExploded)
        {
            if (debugLog) Debug.Log($"{name}: Already exploded, skipping.");
            return;
        }

        if (debugLog) Debug.Log($"{name}: TriggerExplosionDeath called.");
        StartCoroutine(ExplosionDeathRoutine());
    }

    private IEnumerator ExplosionDeathRoutine()
    {
        hasExploded = true;

        if (debugLog) Debug.Log($"{name}: Waiting {explodeDelay} seconds before exploding.");
        yield return new WaitForSeconds(explodeDelay);

        Vector3 spawnPos = transform.position + explosionOffset;

        if (explosionPoint != null)
            spawnPos = explosionPoint.position + explosionOffset;

        if (debugLog) Debug.Log($"{name}: Explosion spawn position = {spawnPos}");

        if (explosionVFXPrefab != null)
        {
            Instantiate(explosionVFXPrefab, spawnPos, Quaternion.identity);
            if (debugLog) Debug.Log($"{name}: Explosion VFX spawned.");
        }
        else
        {
            Debug.LogWarning($"{name}: explosionVFXPrefab is NOT assigned.");
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, spawnPos, explosionVolume);
            if (debugLog) Debug.Log($"{name}: Explosion sound played.");
        }

        if (destroyWholeRootObject)
        {
            if (debugLog) Debug.Log($"{name}: Destroying root object: {transform.root.name}");
            Destroy(transform.root.gameObject);
        }
        else
        {
            if (debugLog) Debug.Log($"{name}: Destroying this object: {name}");
            Destroy(gameObject);
        }
    }
}