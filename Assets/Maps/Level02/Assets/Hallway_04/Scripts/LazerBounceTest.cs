using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBounceTest : MonoBehaviour
{
    [Header("Laser Points")]
    public Transform bouncePoint;
    public Transform targetPoint;

    [Header("Laser State")]
    public bool laserActive = false;

    [Header("Optional Visual End")]
    public GameObject targetHitEffect;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 3;
        UpdateLaserVisual();
    }

    private void Update()
    {
        UpdateLaserVisual();
    }

    public void ActivateLaser()
    {
        laserActive = true;
        UpdateLaserVisual();
    }

    public void DeactivateLaser()
    {
        laserActive = false;
        UpdateLaserVisual();
    }

    public void ToggleLaser()
    {
        laserActive = !laserActive;
        UpdateLaserVisual();
    }

    private void UpdateLaserVisual()
    {
        if (!laserActive || bouncePoint == null || targetPoint == null)
        {
            lineRenderer.enabled = false;

            if (targetHitEffect != null)
                targetHitEffect.SetActive(false);

            return;
        }

        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, bouncePoint.position);
        lineRenderer.SetPosition(2, targetPoint.position);

        if (targetHitEffect != null)
        {
            targetHitEffect.SetActive(true);
            targetHitEffect.transform.position = targetPoint.position;
        }
    }
}