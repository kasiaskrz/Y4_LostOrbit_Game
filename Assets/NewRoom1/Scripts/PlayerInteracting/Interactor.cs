using UnityEngine;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    [Header("Raycast")]
    public Camera cam;
    public float interactDistance = 3.5f;
    public LayerMask interactMask = ~0;
    public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

    [Header("UI")]
    public Image crosshair;
    public Color idleColor = Color.white;
    public Color highlightColor = Color.green;

    public static IInteractable CurrentInteractable { get; private set; }

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        FindInteractable();
        UpdateCrosshair();
    }

    void FindInteractable()
    {
        CurrentInteractable = null;

        if (cam == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask, triggerInteraction))
        {
            CurrentInteractable = hit.collider.GetComponentInParent<IInteractable>();
        }
    }

    void UpdateCrosshair()
    {
        if (crosshair == null) return;
        crosshair.color = (CurrentInteractable != null) ? highlightColor : idleColor;
    }
}