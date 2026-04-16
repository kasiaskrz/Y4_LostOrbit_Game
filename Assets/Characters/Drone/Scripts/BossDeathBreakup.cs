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

    private bool hasDied = false;

    public void PlayDeath()
    {
        if (hasDied) return;
        hasDied = true;

        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

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

        StartCoroutine(DestroyRootAfterDelay(0.05f));
    }

    private IEnumerator DestroyRootAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}