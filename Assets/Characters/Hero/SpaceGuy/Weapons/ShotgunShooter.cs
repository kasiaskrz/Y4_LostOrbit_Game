using UnityEngine;

public class ShotgunShooter : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;              // Player uses this
    public Transform aimTransform;  // Enemy uses this
    public Transform muzzle;
    public ShotTracer tracerPrefab;

    [Header("Inventory Ammo")]
    [Tooltip("Ammo item in your inventory, e.g. shotgun shells.")]
    public ItemData ammoItemData;

    [Tooltip("Optional. If left empty, will use InventoryManager.Instance.")]
    public InventoryManager inventoryManager;

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

    [Header("Loaded Ammo")]
    public int magSize = 6;
    public int currentAmmo = 6;
    public bool useAmmo = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip fireClip;
    public AudioClip emptyClip;
    public AudioClip reloadStartClip;
    public AudioClip reloadInsertClip;
    public AudioClip reloadSingleClip;
    public AudioClip reloadEndClip; // optional / legacy

    [Header("Audio Pitch")]
    public float minPitch = 0.98f;
    public float maxPitch = 1.02f;

    [Header("Audio Cooldowns")]
    public float emptySoundCooldown = 0.15f;

    private float nextEmptySoundTime;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance;

        currentAmmo = Mathf.Clamp(currentAmmo, 0, magSize);
    }

    public void FireOnce()
    {
        if (!muzzle) return;

        if (useAmmo && currentAmmo <= 0)
        {
            PlayEmptySound();
            return;
        }

        Transform aimSource = null;

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
                    if (dmg != null)
                        dmg.TakeDamage(damagePerPellet);
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

    void PlayClip(AudioClip clip, bool randomizePitch = true)
    {
        if (audioSource == null || clip == null) return;

        float originalPitch = audioSource.pitch;

        if (randomizePitch)
            audioSource.pitch = Random.Range(minPitch, maxPitch);

        audioSource.PlayOneShot(clip);
        audioSource.pitch = originalPitch;
    }

    public void PlayFireSound()
    {
        PlayClip(fireClip);
    }

    public void PlayEmptySound()
    {
        if (Time.time < nextEmptySoundTime) return;

        nextEmptySoundTime = Time.time + emptySoundCooldown;
        PlayClip(emptyClip, false);
    }

    public void PlayReloadStartSound()
    {
        PlayClip(reloadStartClip);
    }

    public void PlayReloadInsertSound()
    {
        PlayClip(reloadInsertClip);
    }

    public void PlayReloadSingleSound()
    {
        if (reloadSingleClip != null)
            PlayClip(reloadSingleClip);
        else
            PlayClip(reloadInsertClip);
    }

    public void PlayReloadEndSound()
    {
        PlayClip(reloadEndClip);
    }

    public bool InsertOneShell()
    {
        if (!useAmmo) return false;
        if (currentAmmo >= magSize) return false;
        if (ammoItemData == null) return false;

        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance;

        if (inventoryManager == null) return false;
        if (!inventoryManager.TryRemoveItem(ammoItemData, 1)) return false;

        currentAmmo++;
        return true;
    }

    public bool TryInsertOneShell()
    {
        return InsertOneShell();
    }

    public bool IsMagazineFull()
    {
        return currentAmmo >= magSize;
    }

    public bool HasReserveAmmo()
    {
        return GetReserveAmmo() > 0;
    }

    public int GetMissingShells()
    {
        if (!useAmmo) return 0;
        return Mathf.Max(0, magSize - currentAmmo);
    }

    public bool CanReload()
    {
        if (!useAmmo) return false;
        if (currentAmmo >= magSize) return false;
        if (GetReserveAmmo() <= 0) return false;
        return true;
    }

    public int GetReserveAmmo()
    {
        if (ammoItemData == null)
            return 0;

        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance;

        if (inventoryManager == null)
            return 0;

        return inventoryManager.CountItem(ammoItemData);
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMagSize()
    {
        return magSize;
    }

    public void SetCurrentAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(amount, 0, magSize);
    }

    public void AddAmmoToMagazine(int amount)
    {
        if (!useAmmo || amount <= 0) return;
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, magSize);
    }
}

public interface IDamageable
{
    void TakeDamage(float amount);
}