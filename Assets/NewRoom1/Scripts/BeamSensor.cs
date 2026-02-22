using UnityEngine;

public class BeamSensor : MonoBehaviour
{
    [Header("Visual")]
    public Renderer targetRenderer;            // drag the mesh renderer you want recolored
    public Color idleColor = Color.red;
    public Color hitColor  = Color.green;

    [Header("Optional glow")]
    public bool useEmission = true;
    public float emissionIntensity = 2f;

    [Header("State")]
    [SerializeField] public bool isActive;

    private MaterialPropertyBlock mpb;

    void OnEnable()
    {
        // Make sure visuals initialize even if Awake didn’t run for some reason
        Deactivate();
    }

    void ApplyColor(Color c)
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer == null) return;

        // ✅ prevents "Parameter name: dest"
        if (mpb == null) mpb = new MaterialPropertyBlock();

        targetRenderer.GetPropertyBlock(mpb);

        mpb.SetColor("_BaseColor", c);
        mpb.SetColor("_Color", c);

        if (useEmission)
            mpb.SetColor("_EmissionColor", c * emissionIntensity);

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
