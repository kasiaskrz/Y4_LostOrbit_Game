using UnityEngine;

public class LaserTestButton : MonoBehaviour
{
    [Header("Interaction")]
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactLayer = ~0;

    [Header("Laser")]
    public LaserBounceTest linkedLaser;
    public bool oneUseOnly = true;

    private bool hasBeenUsed = false;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (oneUseOnly && hasBeenUsed)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                PressButton();
            }
        }
    }

    private void PressButton()
    {
        if (linkedLaser == null)
        {
            Debug.LogWarning("No LaserBounceTest assigned.");
            return;
        }

        linkedLaser.ActivateLaser();
        hasBeenUsed = true;

        Debug.Log("Button pressed. Laser activated.");
    }
}