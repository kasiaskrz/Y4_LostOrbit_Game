using UnityEngine;

public class KeyPickup2 : MonoBehaviour, IInteractable
{
    public static bool KeyCollected = false;

    [Header("Inventory")]
    public ItemData keyItemData;

    public void Interact()
    {
        KeyCollected = true;
        Debug.Log("[Key] Collected!");

        if (keyItemData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.TryAddItem(keyItemData, 1);
            // Show pickup notification
            PickupNotification.Show(keyItemData.icon, keyItemData.itemName, 1);
        }

        gameObject.SetActive(false);
    }
}