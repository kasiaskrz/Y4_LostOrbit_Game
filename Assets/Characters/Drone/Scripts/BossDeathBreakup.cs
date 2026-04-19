using System.Collections;
using UnityEngine;

public class BossDeathBreakup : MonoBehaviour
{
    [Header("Parts To Break Off")]
    public Transform[] parts;

    [Header("Explosion")]
    public GameObject explosionVFX;
    public AudioSource audioSource;
    public AudioClip explosionSound;

    [Header("Force")]
    public float explosionForce = 8f;
    public float explosionRadius = 5f;
    public float upwardForce = 1.5f;
    public float torqueForce = 8f;

    [Header("Cleanup")]
    public float destroyDelay = 6f;

    [Header("Win Scene")]
    public string winSceneName = "WinScene";
    public float winDelay = 3f;

    private bool hasDied = false;

    public void PlayDeath()
    {
        if (hasDied) return;
        hasDied = true;

        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        if (audioSource != null && explosionSound != null)
            audioSource.PlayOneShot(explosionSound);

        // Stop timer and submit Level 3 time
        LevelRunReporter reporter = FindFirstObjectByType<LevelRunReporter>();
        if (reporter != null)
            reporter.StopTimer();
        else
            Debug.LogWarning("LevelRunReporter not found in scene!");

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == null) continue;

            Transform part = parts[i];
            part.SetParent(null, true);

            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb == null)
                rb = part.gameObject.AddComponent<Rigidbody>();

            rb.useGravity = true;
            rb.mass = 1f;

            rb.AddExplosionForce(
                explosionForce,
                transform.position,
                explosionRadius,
                upwardForce,
                ForceMode.Impulse
            );

            rb.AddTorque(Random.insideUnitSphere * torqueForce, ForceMode.Impulse);

            Destroy(part.gameObject, destroyDelay);
        }

        // ✅ Single coroutine handles everything — no Invoke that gets cancelled
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(0.05f);

        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;

        yield return new WaitForSeconds(winDelay - 0.05f);

        LevelRunReporter reporter = FindFirstObjectByType<LevelRunReporter>();
        if (reporter != null)
            PlayerPrefs.SetFloat("WinTime", reporter.elapsed);

        UnityEngine.SceneManagement.SceneManager.LoadScene(winSceneName);
    }
}