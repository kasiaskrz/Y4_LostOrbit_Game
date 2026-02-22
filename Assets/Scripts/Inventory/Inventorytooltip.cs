using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// Floating tooltip shown when hovering a slot. Place once in your Canvas.

public class InventoryTooltip : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;
    public Image typeTagImage;
    public TextMeshProUGUI typeTagText;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        Hide();
    }

    public void Show(ItemData data, Vector3 worldPosition)
    {
        gameObject.SetActive(true);

        nameText.text = data.itemName;
        descriptionText.text = data.description;

        if (iconImage != null) iconImage.sprite = data.icon;

        if (typeTagText != null)
            typeTagText.text = data.itemType.ToString().ToUpper();

        // Position near the slot
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPoint, canvas.worldCamera, out Vector2 localPoint);

        rectTransform.localPosition = localPoint + new Vector2(10, -10);
    }

    public void Hide() => gameObject.SetActive(false);
}