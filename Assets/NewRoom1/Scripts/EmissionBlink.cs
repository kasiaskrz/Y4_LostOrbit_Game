using UnityEngine;

public class EmissionBlink : MonoBehaviour
{
    [Header("Assign the material used by the glowing text/sign")]
    public Material mat;

    [Header("Emission settings")]
    public Color emissionColor = Color.cyan;
    public float onIntensity = 2.5f;
    public float offIntensity = 0f;

    [Header("Timing")]
    public float onDuration = 0.25f;   // how long it stays lit
    public float offDuration = 2.0f;   // time between flashes

    int emissionId;

    void Awake()
    {
        emissionId = Shader.PropertyToID("_EmissionColor");

        if (mat != null)
            mat.EnableKeyword("_EMISSION");
    }

    void OnEnable()
    {
        if (mat != null)
            StartCoroutine(BlinkLoop());
    }

    System.Collections.IEnumerator BlinkLoop()
    {
        while (true)
        {
            SetEmission(onIntensity);
            yield return new WaitForSeconds(onDuration);

            SetEmission(offIntensity);
            yield return new WaitForSeconds(offDuration);
        }
    }

    void SetEmission(float intensity)
    {
        if (mat == null) return;
        mat.SetColor(emissionId, emissionColor * intensity);
    }
}
