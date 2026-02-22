using UnityEngine;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    [Header("Raycast")]
    public Camera cam;
    public float interactDistance = 3.5f;
    public LayerMask interactMask = ~0;
    public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public Image crosshair;
    public Color idleColor = Color.white;
    public Color highlightColor = Color.green;
    public ShotgunViewmodelController weapon; // drag it here (from FPS_Viewmodel)

    IInteractable current;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        FindInteractable();
        UpdateCrosshair();

        if (current != null && Input.GetKeyDown(interactKey))
        {
            current.Interact();
        }
    }

    void FindInteractable()
    {
        current = null;

        if (cam == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask, triggerInteraction))
        {
            // supports colliders on child objects
            current = hit.collider.GetComponentInParent<IInteractable>();
        }
    }

    void UpdateCrosshair()
    {
        if (crosshair == null) return;
        crosshair.color = (current != null) ? highlightColor : idleColor;
    }

    // Optional: useful for debugging what you're aiming at
    void OnDrawGizmosSelected()
    {
        if (cam == null) return;
        Gizmos.color = Color.cyan;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Gizmos.DrawRay(ray.origin, ray.direction * interactDistance);
    }
}
