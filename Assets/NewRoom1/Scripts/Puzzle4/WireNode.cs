using UnityEngine;
using UnityEngine.UI;

public class WireNode : MonoBehaviour
{
    public int nodeIndex;
    public bool isLeft;

    private Color baseColor;
    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        baseColor = img.color;
    }

    public Color GetBaseColor() => baseColor;

    public void SetHighlight(bool on)
    {
        img.color = on ? Color.yellow : baseColor;
    }

    public void SetConnected(Color wireColor)
    {
        img.color = wireColor;
    }

    public void Reset()
    {
        img.color = baseColor;
    }
}