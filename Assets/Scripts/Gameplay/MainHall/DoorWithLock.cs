using UnityEngine;

public class DoorWithLock : MonoBehaviour
{
    public Renderer doorRenderer;
    public Color lockedColor = Color.black;
    public Color unlockedColor = Color.white;

    public TeleportOnTrigger teleporter;

    [Header("Inventory")]
    public ItemData keyItemData; // drag your Key ItemData asset here

    private void Start()
    {
        Debug.Log("DOOR-LOCK SCRIPT INITIALIZED");

        if (teleporter != null)
        {
            Debug.Log("Disabling teleporter at start");
            teleporter.enabled = false;
        }

        doorRenderer.material.color = lockedColor;

        // Automatically unlock if key was collected earlier
        if (GameProgress.Instance != null && GameProgress.Instance.keyCollected)
        {
            Debug.Log("Key already collected - unlocking door automatically");
            UnlockDoor();
        }
    }

    public void UnlockDoor()
    {
        Debug.Log("UNLOCKDOOR() CALLED");

        doorRenderer.material.color = unlockedColor;

        // Remove key from inventory when door is unlocked
        if (keyItemData != null && InventoryManager.Instance != null)
            InventoryManager.Instance.TryRemoveItem(keyItemData, 1);

        if (teleporter != null)
        {
            Debug.Log("Enabling teleporter now");
            teleporter.enabled = true;
        }
    }
}