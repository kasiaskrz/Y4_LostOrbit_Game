using System.Collections;
using UnityEngine;

public class ShotgunViewmodelController : MonoBehaviour, IWeaponUIProvider
{
    [Header("Animators")]
    public Animator armsAnimator;
    public Animator gunAnimator;

    [Header("Shooter (optional)")]
    public ShotgunShooter shooter;

    [Header("Ammo")]
    public int magSize = 6;
    public int ammoInMag = 6;

    [Header("Inventory Ammo Item")]
    public ItemData ammoItemData;

    [Header("Input")]
    public KeyCode inspectKey = KeyCode.F;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode reloadKey = KeyCode.R;

    [Header("Animator Params (MATCH YOUR ANIMATOR)")]
    public string fireTrigger = "Fire";
    public string inspectTrigger = "Inspect";
    public string interactTrigger = "Interact";

    public string reloadSingleFullTrigger = "Reload";
    public string reloadStartTrigger = "ReloadStart";
    public string reloadRepeatTrigger = "ReloadRepeat";
    public string reloadEndTrigger = "ReloadEnd"; // kept only so inspector doesn't break
    public string reloadSingleBool = "ReloadSingle";

    [Header("Timing (tweak to match your clips)")]
    public float fireLockTime = 0.65f;
    public float inspectLockTime = 1.0f;
    public float interactLockTime = 0.6f;

    public float reloadStartTime = 0.35f;
    public float reloadRepeatTime = 0.55f;
    public float reloadSingleTime = 0.55f;
    public float reloadEndTime = 0.35f; // unused now, kept so inspector doesn't break

    [Range(0.05f, 0.95f)]
    public float shellInsertPoint = 0.6f;

    private bool busy;
    private bool inventoryReady = false;
    private Coroutine reloadCo;

    // === UI Provider ===
    public int CurrentAmmo => ammoInMag;
    public int MaxAmmo => magSize;
    public bool IsReloading => busy;
    public AmmoVisualType AmmoType => AmmoVisualType.ShotgunShell;

    IEnumerator Start()
    {
        yield return null;
        yield return null;
        inventoryReady = true;
    }

    void Update()
    {
        if (!armsAnimator) return;
        if (Time.timeScale == 0f) return;

        if (!busy && Input.GetMouseButtonDown(0))
            TryFire();

        if (!busy && Input.GetKeyDown(inspectKey))
        {
            TriggerBoth(inspectTrigger);
            StartCoroutine(LockFor(inspectLockTime));
        }

        if (!busy && Input.GetKeyDown(interactKey))
        {
            if (Interactor.CurrentInteractable != null)
            {
                TriggerBoth(interactTrigger);
                Interactor.CurrentInteractable.Interact();
                StartCoroutine(LockFor(interactLockTime));
            }
        }

        if (!busy && Input.GetKeyDown(reloadKey))
            TryReload();
    }

    void TryFire()
    {
        if (ammoInMag <= 0)
        {
            if (shooter) shooter.PlayEmptySound();
            TryReload();
            return;
        }

        ammoInMag--;

        if (shooter)
        {
            shooter.currentAmmo = ammoInMag;
            shooter.FireOnce();
            ammoInMag = shooter.currentAmmo;
        }

        TriggerBoth(fireTrigger);
        StartCoroutine(LockFor(fireLockTime));
    }

    void TryReload()
    {
        if (!inventoryReady) return;

        int missing = magSize - ammoInMag;
        if (missing <= 0) return;

        if (ammoItemData != null && InventoryManager.Instance != null)
        {
            if (!InventoryManager.Instance.HasItem(ammoItemData, 1))
            {
                Debug.Log("[Gun] No ammo in inventory to reload!");
                return;
            }
        }

        busy = true;

        if (reloadCo != null)
            StopCoroutine(reloadCo);

        reloadCo = StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        int shellsNeeded = magSize - ammoInMag;

        if (shellsNeeded <= 0)
        {
            busy = false;
            reloadCo = null;
            yield break;
        }

        // 1 shell missing = just play Reload
        if (shellsNeeded == 1)
        {
            yield return StartCoroutine(PlayReloadSingle());
            busy = false;
            reloadCo = null;
            yield break;
        }

        // 2+ shells missing:
        // ReloadStart = setup only, NO shell inserted here
        SetBoolBoth(reloadSingleBool, false);
        TriggerBoth(reloadStartTrigger);
        yield return new WaitForSeconds(reloadStartTime);

        // After ReloadStart, load shells.
        // All middle shells = ReloadRepeat
        // Final shell = Reload
        while (ammoInMag < magSize)
        {
            shellsNeeded = magSize - ammoInMag;

            if (shellsNeeded <= 0)
                break;

            if (ammoItemData != null && InventoryManager.Instance != null)
            {
                if (!InventoryManager.Instance.HasItem(ammoItemData, 1))
                {
                    Debug.Log("[Gun] Ran out of ammo mid-reload!");
                    break;
                }
            }

            // Final shell uses Reload
            if (shellsNeeded == 1)
            {
                yield return StartCoroutine(PlayReloadSingle());
                break;
            }

            // Otherwise use ReloadRepeat
            TriggerBoth(reloadRepeatTrigger);
            if (shooter) shooter.PlayReloadInsertSound();
            yield return StartCoroutine(InsertShellDuringClip(reloadRepeatTime));
        }

        SetBoolBoth(reloadSingleBool, false);
        busy = false;
        reloadCo = null;
    }

    IEnumerator PlayReloadSingle()
    {
        SetBoolBoth(reloadSingleBool, true);
        TriggerBoth(reloadSingleFullTrigger);
        if (shooter) shooter.PlayReloadSingleSound();

        yield return StartCoroutine(InsertShellDuringClip(reloadSingleTime));

        SetBoolBoth(reloadSingleBool, false);
    }

    IEnumerator InsertShellDuringClip(float clipLength)
    {
        float t1 = Mathf.Clamp01(shellInsertPoint) * clipLength;
        float t2 = clipLength - t1;

        yield return new WaitForSeconds(t1);

        if (ammoInMag < magSize)
        {
            bool hasAmmo = true;

            if (ammoItemData != null && InventoryManager.Instance != null)
            {
                hasAmmo = InventoryManager.Instance.HasItem(ammoItemData, 1);

                if (hasAmmo)
                    InventoryManager.Instance.UseAmmo(ammoItemData, 1);
            }

            if (hasAmmo)
            {
                ammoInMag = Mathf.Clamp(ammoInMag + 1, 0, magSize);

                if (shooter)
                    shooter.currentAmmo = ammoInMag;
            }
        }

        yield return new WaitForSeconds(t2);
    }

    IEnumerator LockFor(float seconds)
    {
        busy = true;
        yield return new WaitForSeconds(seconds);
        busy = false;
    }

    void TriggerBoth(string trig)
    {
        armsAnimator.SetTrigger(trig);
        if (gunAnimator) gunAnimator.SetTrigger(trig);
    }

    void SetBoolBoth(string param, bool value)
    {
        armsAnimator.SetBool(param, value);
        if (gunAnimator) gunAnimator.SetBool(param, value);
    }
}