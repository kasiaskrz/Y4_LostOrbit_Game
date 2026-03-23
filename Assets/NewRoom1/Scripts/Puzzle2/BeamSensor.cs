using UnityEngine;

public class BeamSensor : MonoBehaviour
{
    [Header("Visual")]
    public Renderer targetRenderer;
    public Color idleColor = Color.red;
    public Color hitColor = Color.green;

    [Header("Glow Settings")]
    public bool useEmission = true;
    [Range(0f,10f)]
    public float emissionIntensity = 2.5f;

    [Header("State")]
    [SerializeField] public bool isActive;

    private MaterialPropertyBlock mpb;

    void OnEnable()
    {
        Deactivate();
    }

    void ApplyColor(Color c)
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer == null) return;

        if (mpb == null)
            mpb = new MaterialPropertyBlock();

        targetRenderer.GetPropertyBlock(mpb);

        // Base surface color
        mpb.SetColor("_BaseColor", c);
        mpb.SetColor("_Color", c);

        // Emission (HDR for bloom)
        if (useEmission)
        {
            Color emission = c * emissionIntensity;
            mpb.SetColor("_EmissionColor", emission);
        }

        targetRenderer.SetPropertyBlock(mpb);
    }

    public void Activate()
    {
        isActive = true;
        ApplyColor(hitColor);
    }

    public void Deactivate()
    {
        isActive = false;
        ApplyColor(idleColor);
    }
}