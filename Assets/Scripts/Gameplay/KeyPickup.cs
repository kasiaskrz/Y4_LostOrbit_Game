using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Inventory")]
    public ItemData keyItemData; // drag your Key ItemData asset here

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Mark key as collected
            GameProgress.Instance.keyCollected = true;

            // Add key to inventory
            if (keyItemData != null && InventoryManager.Instance != null)
                InventoryManager.Instance.TryAddItem(keyItemData, 1);

            // Unlock the door
            var door = Object.FindFirstObjectByType<FinishTrigger>();
            if (door != null)
                door.EnableFinishZone();

            // Hide the key after picking up
            gameObject.SetActive(false);

            Debug.Log("KEY COLLECTED!");
        }
    }
}