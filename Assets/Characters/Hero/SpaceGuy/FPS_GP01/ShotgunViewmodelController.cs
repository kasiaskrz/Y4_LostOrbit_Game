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
    public string reloadEndTrigger = "ReloadEnd";
    public string reloadSingleBool = "ReloadSingle";

    [Header("Timing (tweak to match your clips)")]
    public float fireLockTime = 0.15f;
    public float inspectLockTime = 1.0f;
    public float interactLockTime = 0.6f;

    public float reloadStartTime = 0.35f;
    public float reloadRepeatTime = 0.55f;
    public float reloadEndTime = 0.35f;

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
        // Wait two frames for InventoryManager to finish giving starting items
        yield return null;
        yield return null;
        inventoryReady = true;
    }

    void Update()
    {
        if (!armsAnimator) return;

        // Block all gun input when inventory is open
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
            TryReload();
            return;
        }

        ammoInMag--;
        TriggerBoth(fireTrigger);
        if (shooter) shooter.FireOnce();
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

        reloadCo = StartCoroutine(ReloadRoutine(missing));
    }

    IEnumerator ReloadRoutine(int shellsNeeded)
    {
        if (shellsNeeded == 1)
        {
            SetBoolBoth(reloadSingleBool, true);
            TriggerBoth(reloadSingleFullTrigger);

            float t1 = Mathf.Clamp01(shellInsertPoint) * reloadRepeatTime;
            float t2 = reloadRepeatTime - t1;

            yield return new WaitForSeconds(t1);

            if (ammoItemData != null && InventoryManager.Instance != null)
                InventoryManager.Instance.UseAmmo(ammoItemData, 1);

            ammoInMag = Mathf.Clamp(ammoInMag + 1, 0, magSize);
            yield return new WaitForSeconds(t2);

            SetBoolBoth(reloadSingleBool, false);
            busy = false;
            reloadCo = null;
            yield break;
        }

        SetBoolBoth(reloadSingleBool, false);
        TriggerBoth(reloadStartTrigger);
        yield return new WaitForSeconds(reloadStartTime);

        while (shellsNeeded > 0 && ammoInMag < magSize)
        {
            if (ammoItemData != null && InventoryManager.Instance != null)
            {
                if (!InventoryManager.Instance.HasItem(ammoItemData, 1))
                {
                    Debug.Log("[Gun] Ran out of ammo mid-reload!");
                    break;
                }
            }

            TriggerBoth(reloadRepeatTrigger);

            float t1 = Mathf.Clamp01(shellInsertPoint) * reloadRepeatTime;
            float t2 = reloadRepeatTime - t1;

            yield return new WaitForSeconds(t1);

            if (ammoItemData != null && InventoryManager.Instance != null)
                InventoryManager.Instance.UseAmmo(ammoItemData, 1);

            ammoInMag++;
            shellsNeeded--;
            yield return new WaitForSeconds(t2);
        }

        TriggerBoth(reloadEndTrigger);
        yield return new WaitForSeconds(reloadEndTime);

        busy = false;
        reloadCo = null;
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