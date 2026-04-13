using UnityEngine;

public class ElevatorButtonTest : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactLayer = ~0;

    [Header("Fuse Check")]
    public GameObject fuseObject;

    [Header("Bridge")]
    public BridgeController bridgeController;

    public string PromptText =>
        (fuseObject != null && fuseObject.activeSelf)
        ? "Activate Elevator"
        : "No Power";

    public void Interact()
    {

    }

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
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
        if (fuseObject == null)
        {
            Debug.LogWarning("Fuse object not assigned.");
            return;
        }

        if (!fuseObject.activeSelf)
        {
            Debug.Log("Elevator has no fuse.");
            return;
        }

        if (bridgeController != null)
        {
            bridgeController.ActivateBridge();
            Debug.Log("Elevator activated.");
        }
    }
}