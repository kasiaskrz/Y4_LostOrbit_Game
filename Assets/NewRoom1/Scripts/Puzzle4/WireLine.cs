using UnityEngine;
using UnityEngine.UI;

public class WireLine : MonoBehaviour
{
    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void DrawLine(Vector2 from, Vector2 to, Color color)
    {
        GetComponent<Image>().color = color;

        Vector2 dir = to - from;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        rect.sizeDelta = new Vector2(distance, 12f); // 6px thick
        rect.anchoredPosition = from + dir * 0.5f;
        rect.localRotation = Quaternion.Euler(0, 0, angle);
    }
}