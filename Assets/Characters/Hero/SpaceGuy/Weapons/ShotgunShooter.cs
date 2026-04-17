using UnityEngine;

public class ShotgunShooter : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;              // Player uses this
    public Transform aimTransform;  // Enemy uses this
    public Transform muzzle;
    public ShotTracer tracerPrefab;

    [Header("Shotgun")]
    public int pellets = 10;
    public float range = 60f;
    public float spreadDegrees = 4.5f;

    [Header("Damage (optional)")]
    public float damagePerPellet = 8f;
    public LayerMask hitMask = ~0;

    [Header("Stylised lines")]
    [Range(0f, 1f)] public float tracerChance = 0.8f;
    public float tracerMuzzleForwardOffset = 0.05f;

    [Header("Ammo")]
    public int magSize = 6;
    public int currentAmmo = 6;
    public int reserveAmmo = 24;
    public bool useAmmo = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip fireClip;
    public AudioClip emptyClip;
    public AudioClip reloadStartClip;
    public AudioClip reloadInsertClip;
    public AudioClip reloadEndClip;

    [Header("Audio Pitch")]
    public float minPitch = 0.98f;
    public float maxPitch = 1.02f;

    public void FireOnce()
    {
        if (!muzzle) return;

        if (useAmmo && currentAmmo <= 0)
        {
            PlayEmptySound();
            return;
        }

        Transform aimSource = null;

        // Decide aim source
        if (cam != null)
            aimSource = cam.transform;
        else if (aimTransform != null)
            aimSource = aimTransform;
        else
            return;

        if (useAmmo)
            currentAmmo--;

        PlayFireSound();

        Vector3 tracerStart = muzzle.position + muzzle.forward * tracerMuzzleForwardOffset;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 dir = GetSpreadDirection(aimSource.forward, aimSource);

            Vector3 end = tracerStart + dir * range;

            // Raycast from aim source
            if (Physics.Raycast(aimSource.position, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
            {
                end = hit.point;

                var bossDmg = hit.collider.GetComponentInParent<IDamageableBoss>();
                if (bossDmg != null)
                {
                    bossDmg.TakeDamage(Mathf.RoundToInt(damagePerPellet));
                }
                else
                {
                    var dmg = hit.collider.GetComponentInParent<IDamageable>();
                    if (dmg != null) dmg.TakeDamage(damagePerPellet);
                }
            }

            if (tracerPrefab && Random.value <= tracerChance)
            {
                ShotTracer tr = Instantiate(tracerPrefab);
                tr.Init(tracerStart, end);
            }
        }
    }

    Vector3 GetSpreadDirection(Vector3 forward, Transform aimSource)
    {
        float angle = spreadDegrees * Mathf.Deg2Rad;
        Vector2 r = Random.insideUnitCircle * Mathf.Tan(angle);

        return (forward +
                aimSource.right * r.x +
                aimSource.up * r.y).normalized;
    }

    void PlayClip(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip);
        audioSource.pitch = 1f;
    }

    public void PlayFireSound()
    {
        PlayClip(fireClip);
    }

    public void PlayEmptySound()
    {
        PlayClip(emptyClip);
    }

    public void PlayReloadStartSound()
    {
        PlayClip(reloadStartClip);
    }

    public void PlayReloadInsertSound()
    {
        PlayClip(reloadInsertClip);
    }

    public void PlayReloadEndSound()
    {
        PlayClip(reloadEndClip);
    }

    public void InsertOneShell()
    {
        if (!useAmmo) return;
        if (reserveAmmo <= 0) return;
        if (currentAmmo >= magSize) return;

        currentAmmo++;
        reserveAmmo--;
    }

    public bool IsMagazineFull()
    {
        return currentAmmo >= magSize;
    }

    public bool HasReserveAmmo()
    {
        return reserveAmmo > 0;
    }
}

public interface IDamageable
{
    void TakeDamage(float amount);
}