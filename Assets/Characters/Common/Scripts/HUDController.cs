using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Refs")]
    public MonoBehaviour weaponProviderBehaviour; // drag your shotgun script here
    public PlayerHealth playerHealth;

    [Header("Ammo UI")]
    public Image ammoIcon;
    public TMP_Text ammoText;

    [Header("Health UI")]
    public Image healthFill;

    [Header("Sprites for ammo types")]
    public Sprite shotgunShellSprite;
    public Sprite rifleBulletSprite;
    public Sprite pistolRoundSprite;
    public Sprite energyCellSprite;

    [Header("Smoothing")]
    public float healthLerpSpeed = 12f;

    IWeaponUIProvider weapon;
    float healthTarget01 = 1f;
    float healthShown01 = 1f;

    void Awake()
    {
        weapon = weaponProviderBehaviour as IWeaponUIProvider;
        if (playerHealth != null)
            playerHealth.OnHealthChanged += OnHealthChanged;
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= OnHealthChanged;
    }

    void Update()
    {
        // AMMO
        if (weapon != null && ammoText != null)
        {
            ammoText.text = $"{weapon.CurrentAmmo:00}<size=60%>/{weapon.MaxAmmo:00}</size>";
            UpdateAmmoIcon(weapon.AmmoType);
        }

        // HEALTH (lerp)
        if (healthFill != null)
        {
            healthShown01 = Mathf.Lerp(healthShown01, healthTarget01, Time.deltaTime * healthLerpSpeed);
            healthFill.fillAmount = healthShown01;
        }
    }

    void OnHealthChanged(int current, int max)
    {
        healthTarget01 = (max <= 0) ? 0f : (float)current / max;
        healthTarget01 = Mathf.Clamp01(healthTarget01);
    }

    void UpdateAmmoIcon(AmmoVisualType type)
    {
        if (!ammoIcon) return;

        Sprite s = null;
        switch (type)
        {
            case AmmoVisualType.ShotgunShell: s = shotgunShellSprite; break;
            case AmmoVisualType.RifleBullet: s = rifleBulletSprite; break;
            case AmmoVisualType.PistolRound: s = pistolRoundSprite; break;
            case AmmoVisualType.EnergyCell:  s = energyCellSprite; break;
            default: s = null; break;
        }

        ammoIcon.enabled = (s != null);
        ammoIcon.sprite = s;
    }
}
