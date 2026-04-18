using UnityEngine;
public class InstantPickupItem : MonoBehaviour
{
    public enum PickupType
    {
        Health,
        Ammo
    }
    public PickupType pickupType;
    [Header("Health")]
    public int healAmount = 25;
    [Header("Ammo")]
    public AmmoVisualType ammoType;
    public int ammoAmount = 10;
    [Header("Inventory")]
    public ItemData ammoItemData;
    [Header("FX")]
    public GameObject pickupEffect;
    [Header("Audio")]
    public AudioClip pickupSound;
    [Range(0f, 1f)]
    public float volume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        // HEALTH
        if (pickupType == PickupType.Health)
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.Heal(healAmount);
        }
        // AMMO
        if (pickupType == PickupType.Ammo)
        {
            PlayerAmmo ammo = other.GetComponent<PlayerAmmo>();
            if (ammo != null)
                ammo.AddAmmo(ammoType, ammoAmount);

            if (ammoItemData != null && InventoryManager.Instance != null)
                InventoryManager.Instance.TryAddItem(ammoItemData, ammoAmount);
        }
        // Spawn effect (optional)
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        // Play sound
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);

        Destroy(gameObject);
    }
}