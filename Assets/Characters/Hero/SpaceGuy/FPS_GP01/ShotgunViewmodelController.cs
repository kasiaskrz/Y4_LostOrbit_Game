using System.Collections;
using UnityEngine;
public class ShotgunViewmodelController : MonoBehaviour, IWeaponUIProvider
{
    public Animator armsAnimator;
    public Animator gunAnimator;
    public ShotgunShooter shooter;
    public string fireTrigger = "Fire";
    public string inspectTrigger = "Inspect";
    public string interactTrigger = "Interact";
    public string reloadSingleFullTrigger = "Reload";
    public string reloadStartTrigger = "ReloadStart";
    public string reloadRepeatTrigger = "ReloadRepeat";
    public string reloadEndTrigger = "ReloadEnd";
    public string reloadSingleBool = "ReloadSingle";
    public float fireLockTime = 0.65f;
    public float inspectLockTime = 1.0f;
    public float interactLockTime = 0.6f;
    public float reloadStartTime = 0.35f;
    public float reloadRepeatTime = 0.55f;
    public float reloadSingleTime = 0.55f;
    public float reloadEndTime = 0.35f;
    [Range(0.05f, 0.95f)] public float shellInsertPoint = 0.6f;
    private bool busy;
    private bool inventoryReady = false;
    private Coroutine reloadCo;
    public int CurrentAmmo => shooter != null ? shooter.GetCurrentAmmo() : 0;
    public int MaxAmmo => shooter != null ? shooter.GetMagSize() : 0;
    public bool IsReloading => busy;
    public AmmoVisualType AmmoType => AmmoVisualType.ShotgunShell;
    IEnumerator Start()
    {
        yield return null; yield return null;
        inventoryReady = true;
        if (shooter == null) shooter = GetComponent<ShotgunShooter>();
    }
    void Update()
    {
        if (!armsAnimator || Time.timeScale == 0f || shooter == null) return;
        if (!busy && Input.GetMouseButtonDown(0)) TryFire();
        if (!busy && Input.GetKeyDown(OptionsManager.Inspect)) { TriggerBoth(inspectTrigger); StartCoroutine(LockFor(inspectLockTime)); }
        if (!busy && Input.GetKeyDown(OptionsManager.Interact) && Interactor.CurrentInteractable != null)
        { TriggerBoth(interactTrigger); Interactor.CurrentInteractable.Interact(); StartCoroutine(LockFor(interactLockTime)); }
        if (!busy && Input.GetKeyDown(OptionsManager.Reload)) TryReload();
    }
    void TryFire()
    {
        if (shooter == null) return;
        if (shooter.GetCurrentAmmo() <= 0) { shooter.PlayEmptySound(); TryReload(); return; }
        shooter.FireOnce(); TriggerBoth(fireTrigger); StartCoroutine(LockFor(fireLockTime));
    }
    void TryReload()
    {
        if (!inventoryReady || shooter == null || !shooter.CanReload()) return;
        busy = true;
        if (reloadCo != null) StopCoroutine(reloadCo);
        reloadCo = StartCoroutine(ReloadRoutine());
    }
    IEnumerator ReloadRoutine()
    {
        if (shooter == null) { busy = false; reloadCo = null; yield break; }
        int n = shooter.GetMissingShells();
        if (n <= 0) { busy = false; reloadCo = null; yield break; }
        if (n == 1) { yield return StartCoroutine(PlayReloadSingle()); busy = false; reloadCo = null; yield break; }
        SetBoolBoth(reloadSingleBool, false); TriggerBoth(reloadStartTrigger);
        if (shooter != null) shooter.PlayReloadStartSound();
        yield return new WaitForSeconds(reloadStartTime);
        while (shooter != null && shooter.CanReload())
        {
            n = shooter.GetMissingShells(); if (n <= 0) break;
            if (n == 1) { yield return StartCoroutine(PlayReloadSingle()); break; }
            TriggerBoth(reloadRepeatTrigger); if (shooter != null) shooter.PlayReloadInsertSound();
            yield return StartCoroutine(InsertShellDuringClip(reloadRepeatTime));
        }
        SetBoolBoth(reloadSingleBool, false); busy = false; reloadCo = null;
    }
    IEnumerator PlayReloadSingle()
    {
        if (shooter == null) yield break;
        SetBoolBoth(reloadSingleBool, true); TriggerBoth(reloadSingleFullTrigger);
        shooter.PlayReloadSingleSound();
        yield return StartCoroutine(InsertShellDuringClip(reloadSingleTime));
        SetBoolBoth(reloadSingleBool, false);
    }
    IEnumerator InsertShellDuringClip(float len)
    {
        float t1 = Mathf.Clamp01(shellInsertPoint) * len;
        yield return new WaitForSeconds(t1);
        if (shooter != null && shooter.CanReload()) shooter.TryInsertOneShell();
        yield return new WaitForSeconds(len - t1);
    }
    IEnumerator LockFor(float s) { busy = true; yield return new WaitForSeconds(s); busy = false; }
    void TriggerBoth(string t) { if (armsAnimator) armsAnimator.SetTrigger(t); if (gunAnimator) gunAnimator.SetTrigger(t); }
    void SetBoolBoth(string p, bool v) { if (armsAnimator) armsAnimator.SetBool(p, v); if (gunAnimator) gunAnimator.SetBool(p, v); }
}