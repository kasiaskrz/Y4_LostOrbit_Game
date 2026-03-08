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

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip pressSound;
    [Range(0.8f, 1.2f)] public float pitchVariation = 0.05f;

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

        // Play sound
        if (audioSource != null && pressSound != null)
        {
            audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
            audioSource.PlayOneShot(pressSound);
        }

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

    public void SetLightColor(Color c)
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