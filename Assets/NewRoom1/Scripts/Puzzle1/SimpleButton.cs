using UnityEngine;

public class SimpleButton : MonoBehaviour, IInteractable
{
    public bool pressed = false;

    [Header("Optional Door Hookup (old system)")]
    public DoorsUnlockedButtons door;

    [Header("Optional Puzzle Manager (new system)")]
    public ButtonSequencePuzzle puzzle;

    [Header("Visual")]
    public Renderer lightRenderer;
    public Color offColor = Color.red;
    public Color onColor = Color.green;

    [Header("Emission")]
    public bool useEmission = true;
    public float emissionIntensity = 2f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip pressSound;
    [Range(0.8f, 1.2f)] public float pitchVariation = 0.05f;

    MaterialPropertyBlock mpb;

    public string PromptText => pressed ? "" : "Press Button";

    void OnEnable()
    {
        ResetVisual();
    }

    public void Interact() => Press();

    public void Press()
    {
        if (pressed) return;

        pressed = true;

        if (audioSource && pressSound)
        {
            audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
            audioSource.PlayOneShot(pressSound);
        }

        ApplyColor(onColor);

        if (door != null)
            door.ButtonPressed(this);

        if (puzzle != null)
            puzzle.OnButtonPressed(this);
    }

    public void ResetVisual()
    {
        pressed = false;
        ApplyColor(offColor);
    }

    public void ResetButton() => ResetVisual();

    void ApplyColor(Color c)
    {
        if (lightRenderer == null)
            lightRenderer = GetComponentInChildren<Renderer>();

        if (lightRenderer == null) return;

        if (mpb == null)
            mpb = new MaterialPropertyBlock();

        lightRenderer.GetPropertyBlock(mpb);

        mpb.SetColor("_BaseColor", c);
        mpb.SetColor("_Color", c);

        if (useEmission)
            mpb.SetColor("_EmissionColor", c * emissionIntensity);

        lightRenderer.SetPropertyBlock(mpb);
    }
}