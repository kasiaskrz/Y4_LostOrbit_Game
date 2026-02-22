using UnityEngine;

public class PillarFadeOnPlayer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float fadedAlpha = 0.3f;
    public float fadeSpeed = 5f;

    private Renderer[] renderers;
    private Material[] materials;
    private float targetAlpha = 1f;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        materials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            // Clone instance material so we don't affect all objects using this material
            materials[i] = renderers[i].material;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            targetAlpha = fadedAlpha;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            targetAlpha = 1f;
    }

    void Update()
    {
        foreach (var mat in materials)
        {
            if (mat == null) continue;

            // URP Simple Lit uses _BaseColor, not mat.color
            Color c = mat.GetColor("_BaseColor");
            float newAlpha = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            c.a = newAlpha;
            mat.SetColor("_BaseColor", c);
        }
    }
}
