using UnityEngine;

public class DoorWithLock : MonoBehaviour
{
    public Renderer doorRenderer;
    public Color lockedColor = Color.black;
    public Color unlockedColor = Color.white;

    public TeleportOnTrigger teleporter;

    [Header("Inventory")]
    public ItemData keyItemData;

    [Header("UI Popup")]
    [Tooltip("Assign MainHallGuide to show locked message when player tries to enter.")]
    public MainHallGuide mainHallGuide;

    private bool isUnlocked = false;

    private void Start()
    {
        if (teleporter != null)
            teleporter.enabled = false;

        doorRenderer.material.color = lockedColor;

        if (GameProgress.Instance != null && GameProgress.Instance.CanAccessSC005)
        {
            UnlockDoor();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isUnlocked) return;

        // Player tried to enter but door is locked
        if (mainHallGuide != null)
            mainHallGuide.ShowLockedDoorMessage();
    }

    public void UnlockDoor()
    {
        isUnlocked = true;
        doorRenderer.material.color = unlockedColor;

        if (keyItemData != null && InventoryManager.Instance != null)
            InventoryManager.Instance.TryRemoveItem(keyItemData, 1);

        if (teleporter != null)
            teleporter.enabled = true;
    }
}