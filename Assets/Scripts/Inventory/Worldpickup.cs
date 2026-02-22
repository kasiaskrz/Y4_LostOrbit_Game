using UnityEngine;

/// <summary>
/// Attach to any collectable world object (key, ammo crate, etc.)
/// Set itemData and quantity in the Inspector or via code (e.g. when dropped).
/// </summary>
[RequireComponent(typeof(Collider))]
public class WorldPickup : MonoBehaviour
{
    [Header("Item")]
    public ItemData itemData;
    public int quantity = 1;

    [Header("Pickup Settings")]
    public float pickupRadius = 1.5f;          // used if auto-pickup is enabled
    public bool autoPickup = false;            // if true, picks up on trigger enter
    [SerializeField] private string playerTag = "Player";

    [Header("Feedback")]
    public AudioClip pickupSound;
    public GameObject pickupVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (!autoPickup) return;
        if (!other.CompareTag(playerTag)) return;
        Pickup();
    }

    /// <summary>Call this from your player interaction script (e.g. on press E).</summary>
    public void Pickup()
    {
        if (itemData == null) return;

        bool added = InventoryManager.Instance.TryAddItem(itemData, quantity);

        if (added)
        {
            if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            if (pickupVFX)   Instantiate(pickupVFX, transform.position, Quaternion.identity);

            // Show a small HUD notification
            PickupNotification.Show(itemData.icon, itemData.itemName, quantity);

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("[Inventory] Full — could not pick up " + itemData.itemName);
        }
    }

    // Draw pickup radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}