using UnityEngine;

/// <summary>
/// Attach to the Player. Raycasts forward; press E to interact with WorldPickup objects.
/// Requires a Camera reference (assign in Inspector or auto-finds main camera).
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactRange = 2.5f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask pickupLayer;

    [Header("References")]
    public Camera playerCamera;

    [Header("Drop")]
    public KeyCode dropKey = KeyCode.G;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        // Interact / pickup
        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, pickupLayer))
            {
                WorldPickup pickup = hit.collider.GetComponent<WorldPickup>();
                if (pickup != null) pickup.Pickup();
            }
        }

        // Drop selected hotbar item
        if (Input.GetKeyDown(dropKey))
            InventoryManager.Instance.DropSelectedHotbarItem(transform);
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(playerCamera.transform.position,
                       playerCamera.transform.forward * interactRange);
    }
}