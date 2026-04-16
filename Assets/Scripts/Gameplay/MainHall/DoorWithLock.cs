using UnityEngine;

public class DoorWithLock : MonoBehaviour
{
    public Renderer doorRenderer;
    public Color lockedColor = Color.black;
    public Color unlockedColor = Color.white;

    public TeleportOnTrigger teleporter;

    [Header("Inventory")]
    public ItemData keyItemData;

    private void Start()
    {
        Debug.Log("DOOR-LOCK SCRIPT INITIALIZED");

        if (teleporter != null)
        {
            Debug.Log("Disabling teleporter at start");
            teleporter.enabled = false;
        }

        doorRenderer.material.color = lockedColor;

        // Auto unlock if both rooms already complete (e.g. returning from a room)
        if (GameProgress.Instance != null && GameProgress.Instance.CanAccessSC005)
        {
            Debug.Log("Both rooms complete - auto unlocking SC005 door.");
            UnlockDoor();
        }
    }

    public void UnlockDoor()
    {
        Debug.Log("UNLOCKDOOR() CALLED");

        doorRenderer.material.color = unlockedColor;

        if (keyItemData != null && InventoryManager.Instance != null)
            InventoryManager.Instance.TryRemoveItem(keyItemData, 1);

        if (teleporter != null)
        {
            Debug.Log("Enabling teleporter now");
            teleporter.enabled = true;
        }
    }
}