using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Grid")]
    public Transform gridContainer;
    public GameObject slotPrefab;

    [Header("Hotbar")]
    public Transform hotbarContainer;

    [Header("Toggle Key")]
    public KeyCode toggleKey = KeyCode.Tab;

    private InventorySlotUI[] gridSlotUIs;
    private InventorySlotUI[] hotbarSlotUIs;

    private bool isOpen = false;

    private void Start()
    {
        InventoryManager inv = InventoryManager.Instance;

        if (inv == null)
        {
            Debug.LogError("[InventoryUI] InventoryManager not found! Make sure it exists in the scene.");
            return;
        }

        // Build grid slots
        gridSlotUIs = new InventorySlotUI[inv.GridSlots];
        for (int i = 0; i < inv.GridSlots; i++)
        {
            GameObject go = Instantiate(slotPrefab, gridContainer);
            InventorySlotUI slot = go.GetComponent<InventorySlotUI>();
            slot.slotIndex = inv.HotbarSlots + i;
            gridSlotUIs[i] = slot;
        }

        // Build hotbar slots
        hotbarSlotUIs = new InventorySlotUI[inv.HotbarSlots];
        for (int i = 0; i < inv.HotbarSlots; i++)
        {
            GameObject go = Instantiate(slotPrefab, hotbarContainer);
            InventorySlotUI slot = go.GetComponent<InventorySlotUI>();
            slot.slotIndex = i;
            hotbarSlotUIs[i] = slot;
        }

        // Hide grid on start
        gridContainer.gameObject.SetActive(false);

        // Subscribe to events
        inv.OnInventoryChanged += RefreshAll;
        inv.OnHotbarSelectionChanged += RefreshHotbar;

        RefreshAll();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance == null) return;
        InventoryManager.Instance.OnInventoryChanged -= RefreshAll;
        InventoryManager.Instance.OnHotbarSelectionChanged -= RefreshHotbar;
    }

    private void Update()
    {
        if (NotePickup.IsOpen) return; // block only when note is open
        if (Input.GetKeyDown(toggleKey))
            ToggleInventory();
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;
        gridContainer.gameObject.SetActive(isOpen);

        // Pause/unpause game
        Time.timeScale = isOpen ? 0f : 1f;

        // Show/hide cursor
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        Debug.Log("[InventoryUI] Inventory " + (isOpen ? "opened" : "closed"));
    }

    private void RefreshAll()
    {
        RefreshGrid();
        RefreshHotbar(InventoryManager.Instance.SelectedHotbarIndex);
    }

    private void RefreshGrid()
    {
        if (gridSlotUIs == null) return;
        for (int i = 0; i < gridSlotUIs.Length; i++)
        {
            int slotIndex = InventoryManager.Instance.HotbarSlots + i;
            gridSlotUIs[i].Refresh(InventoryManager.Instance.GetSlot(slotIndex));
        }
    }

    private void RefreshHotbar(int selectedIndex)
    {
        if (hotbarSlotUIs == null) return;
        for (int i = 0; i < hotbarSlotUIs.Length; i++)
        {
            hotbarSlotUIs[i].Refresh(
                InventoryManager.Instance.GetSlot(i),
                isSelected: i == selectedIndex
            );
        }
    }
}