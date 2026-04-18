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
    public ItemData ammoItemData; // assign the Ammo ItemData asset in Inspector
    [Header("FX")]
    public GameObject pickupEffect;
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

            // Also add to inventory so it shows in hotbar/grid
            if (ammoItemData != null && InventoryManager.Instance != null)
                InventoryManager.Instance.TryAddItem(ammoItemData, ammoAmount);
        }
        // Spawn effect (optional)
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}