using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour,
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI References")]
    public Image iconImage;
    public Image backgroundImage;
    public TextMeshProUGUI quantityText;
    public Image selectionHighlight;

    [HideInInspector] public int slotIndex;

    private InventoryItem currentItem;
    private InventoryTooltip tooltip;

    // Static drag state shared across all slots
    private static InventorySlotUI dragSource = null;
    private static GameObject dragIcon = null;
    private static Canvas rootCanvas = null;

    private void Awake()
    {
        tooltip = FindFirstObjectByType<InventoryTooltip>();
        if (rootCanvas == null)
            rootCanvas = GetComponentInParent<Canvas>();
    }

    public void Refresh(InventoryItem item, bool isSelected = false)
    {
        currentItem = item;

        bool hasItem = item != null && !item.IsEmpty;

        iconImage.enabled = hasItem;
        quantityText.enabled = hasItem && item.quantity > 1;

        if (hasItem)
        {
            iconImage.sprite = item.data.icon;
            iconImage.preserveAspect = true;  // preserve size
            quantityText.text = item.quantity.ToString();
        }

        if (selectionHighlight != null)
            selectionHighlight.enabled = isSelected;
    }

    // ── Right click to drop ──────────────────────────────────────────────
    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (currentItem != null && !currentItem.IsEmpty)
            {
                Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (playerTransform != null)
                    InventoryManager.Instance.DropSlot(slotIndex, playerTransform);
            }
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Time.unscaledTime - lastClickTime < doubleClickThreshold)
            {
                // Double click detected
                if (currentItem != null && !currentItem.IsEmpty && currentItem.data.itemType == ItemType.Note)
                {
                    NoteReader.Instance.OpenNote(currentItem.data);
                }
            }
            lastClickTime = Time.unscaledTime;
        }
    }

    // ── Tooltip ──────────────────────────────────────────────────────────

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null && currentItem != null && !currentItem.IsEmpty)
            tooltip.Show(currentItem.data, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.Hide();
    }

    // ── Drag and Drop ────────────────────────────────────────────────────

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null || currentItem.IsEmpty) return;

        dragSource = this;

        // Create a floating icon that follows the cursor
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(rootCanvas.transform, false);
        dragIcon.transform.SetAsLastSibling(); // render on top

        RectTransform rt = dragIcon.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(60, 60);

        Image img = dragIcon.AddComponent<Image>();
        img.sprite = currentItem.data.icon;
        img.raycastTarget = false; // so it doesn't block drop targets

        // Hide the icon in the source slot while dragging
        iconImage.enabled = false;
        quantityText.enabled = false;

        if (tooltip != null) tooltip.Hide();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null) return;

        // Move the floating icon with the cursor
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            eventData.position,
            rootCanvas.worldCamera,
            out Vector2 localPoint);

        dragIcon.GetComponent<RectTransform>().localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Clean up the floating icon
        if (dragIcon != null)
        {
            Destroy(dragIcon);
            dragIcon = null;
        }

        // Restore icon visibility if drop didn't happen on a valid slot
        if (dragSource == this)
        {
            Refresh(InventoryManager.Instance.GetSlot(slotIndex));
            dragSource = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (dragSource == null || dragSource == this) return;

        // Swap the two slots
        SwapSlots(dragSource.slotIndex, this.slotIndex);

        dragSource = null;
    }

    // ── Slot Swap Logic ──────────────────────────────────────────────────

    private void SwapSlots(int fromIndex, int toIndex)
    {
        InventoryManager inv = InventoryManager.Instance;

        InventoryItem fromItem = inv.GetSlot(fromIndex);
        InventoryItem toItem = inv.GetSlot(toIndex);


        InventoryManager.Instance.SwapSlots(fromIndex, toIndex);
    }


}