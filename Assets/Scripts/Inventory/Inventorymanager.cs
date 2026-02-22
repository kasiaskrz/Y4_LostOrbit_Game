using System;
using System.Collections;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int gridSlots = 20;
    [SerializeField] private int hotbarSlots = 5;

    [Header("Starting Items")]
    public ItemData startingAmmo;
    public int startingAmmoAmount = 5;

    public event Action OnInventoryChanged;

    private InventoryItem[] slots;

    public int TotalSlots => gridSlots + hotbarSlots;
    public int HotbarSlots => hotbarSlots;
    public int GridSlots => gridSlots;

    public int SelectedHotbarIndex { get; private set; } = 0;
    public event Action<int> OnHotbarSelectionChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        slots = new InventoryItem[TotalSlots];
        for (int i = 0; i < TotalSlots; i++)
            slots[i] = new InventoryItem(null, 0);
    }

    private void Start()
    {
        StartCoroutine(GiveStartingItems());
    }

    private IEnumerator GiveStartingItems()
    {
        yield return null;

        if (startingAmmo != null)
        {
            slots[0] = new InventoryItem(startingAmmo, startingAmmoAmount);
            OnInventoryChanged?.Invoke();
            // No notification for starting items
        }
    }

    // ── Public API ──────────────────────────────────────────────────────────

    /// <summary>Add an item to inventory and show a pickup notification.</summary>
    public bool TryAddItem(ItemData data, int amount = 1, bool showNotification = true)
    {
        if (data == null || amount <= 0) return false;

        int remaining = amount;

        if (data.stackable)
        {
            for (int i = 0; i < TotalSlots && remaining > 0; i++)
            {
                if (slots[i].data == data && slots[i].quantity < data.maxStack)
                {
                    int space = data.maxStack - slots[i].quantity;
                    int add = Mathf.Min(space, remaining);
                    slots[i].quantity += add;
                    remaining -= add;
                }
            }
        }

        for (int i = 0; i < TotalSlots && remaining > 0; i++)
        {
            if (slots[i].IsEmpty)
            {
                int add = data.stackable ? Mathf.Min(data.maxStack, remaining) : 1;
                slots[i] = new InventoryItem(data, add);
                remaining -= add;
            }
        }

        bool addedSome = remaining < amount;
        if (addedSome)
        {
            OnInventoryChanged?.Invoke();

            // Show pickup notification
            if (showNotification)
                PickupNotification.Show(data.icon, data.itemName, amount - remaining);
        }
        return addedSome;
    }

    public bool TryRemoveItem(ItemData data, int amount = 1)
    {
        if (!HasItem(data, amount)) return false;

        int remaining = amount;
        for (int i = TotalSlots - 1; i >= 0 && remaining > 0; i--)
        {
            if (slots[i].data == data)
            {
                int remove = Mathf.Min(slots[i].quantity, remaining);
                slots[i].quantity -= remove;
                remaining -= remove;
                if (slots[i].quantity <= 0)
                    slots[i] = new InventoryItem(null, 0);
            }
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>Swap two slots by index. Works across hotbar and grid.</summary>
    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= TotalSlots) return;
        if (indexB < 0 || indexB >= TotalSlots) return;
        if (indexA == indexB) return;

        InventoryItem temp = slots[indexA];
        slots[indexA] = slots[indexB];
        slots[indexB] = temp;

        OnInventoryChanged?.Invoke();
    }

    /// <summary>Call from gun script when firing to deduct ammo.</summary>
    public bool UseAmmo(ItemData ammoData, int amount = 1)
    {
        return TryRemoveItem(ammoData, amount);
    }

    public void DropSlot(int slotIndex, Transform dropOrigin)
    {
        if (slotIndex < 0 || slotIndex >= TotalSlots) return;
        InventoryItem item = slots[slotIndex];
        if (item.IsEmpty) return;

        if (item.data.worldPrefab != null)
        {
            Vector3 pos = dropOrigin.position + dropOrigin.forward * 1.2f;
            GameObject dropped = Instantiate(item.data.worldPrefab, pos, Quaternion.identity);
            WorldPickup pickup = dropped.GetComponent<WorldPickup>();
            if (pickup != null)
            {
                pickup.itemData = item.data;
                pickup.quantity = item.quantity;
            }
        }

        slots[slotIndex] = new InventoryItem(null, 0);
        OnInventoryChanged?.Invoke();
    }

    public void DropSelectedHotbarItem(Transform dropOrigin)
        => DropSlot(SelectedHotbarIndex, dropOrigin);

    public bool HasItem(ItemData data, int amount = 1)
    {
        int found = 0;
        foreach (var slot in slots)
            if (slot.data == data) found += slot.quantity;
        return found >= amount;
    }

    public int CountItem(ItemData data)
    {
        int total = 0;
        foreach (var slot in slots)
            if (slot.data == data) total += slot.quantity;
        return total;
    }

    public InventoryItem GetSlot(int index) => slots[index];

    // ── Hotbar Selection ────────────────────────────────────────────────────

    public void SelectHotbarSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots) return;
        SelectedHotbarIndex = index;
        OnHotbarSelectionChanged?.Invoke(index);
    }

    private void Update()
    {
        for (int i = 0; i < hotbarSlots; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectHotbarSlot(i);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SelectHotbarSlot((SelectedHotbarIndex - 1 + hotbarSlots) % hotbarSlots);
        else if (scroll < 0f) SelectHotbarSlot((SelectedHotbarIndex + 1) % hotbarSlots);
    }
}