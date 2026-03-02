using UnityEngine;

public class SimpleButton : MonoBehaviour, IInteractable
{
    public bool pressed = false;

    [Header("Optional Door Hookup (old system)")]
    public DoorsUnlockedButtons door;

    [Header("Optional Puzzle Manager (new system)")]
    public ButtonSequencePuzzle puzzle;

    [Header("Light above button (separate object)")]
    public Renderer lightRenderer;
    public Color offColor = Color.red;
    public Color onColor = Color.green;

    public bool useEmission = false;
    public string emissionProperty = "_EmissionColor";

    Material _matInstance;

    public string PromptText => pressed ? "" : "Press Button";

    void Awake()
    {
        if (lightRenderer != null)
        {
            _matInstance = lightRenderer.material;
            SetLightColor(offColor);
        }
    }

    public void Interact() => Press();

    public void Press()
    {
        if (pressed) return;

        pressed = true;
        SetLightColor(onColor);

        if (door != null)
            door.ButtonPressed(this);

        if (puzzle != null)
            puzzle.OnButtonPressed(this);
    }

    public void ResetVisual()
    {
        pressed = false;
        SetLightColor(offColor);
    }

    public void ResetButton() => ResetVisual();

    void SetLightColor(Color c)
    {
        if (_matInstance == null) return;

        if (_matInstance.HasProperty("_Color"))
            _matInstance.color = c;

        if (useEmission && _matInstance.HasProperty(emissionProperty))
        {
            _matInstance.EnableKeyword("_EMISSION");
            _matInstance.SetColor(emissionProperty, c);
        }
    }
}