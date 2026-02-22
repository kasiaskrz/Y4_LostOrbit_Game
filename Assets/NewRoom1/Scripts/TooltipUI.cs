using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public RectTransform tooltipRect;
    public TMP_Text tooltipText;
    public GameObject tooltipPanel;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    void Update()
    {
        if (!tooltipPanel.activeSelf) return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            tooltipRect.parent as RectTransform,
            Input.mousePosition,
            null,
            out pos
        );

        tooltipRect.anchoredPosition = pos + new Vector2(16, -16);
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
