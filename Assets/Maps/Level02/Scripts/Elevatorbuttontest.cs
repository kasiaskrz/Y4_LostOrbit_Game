using UnityEngine;
public class ElevatorButtonTest : MonoBehaviour, IInteractable
{
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public LayerMask interactLayer = ~0;
    public GameObject fuseObject;
    public BridgeController bridgeController;
    public string PromptText => (fuseObject != null && fuseObject.activeSelf) ? "Activate Elevator" : "No Power";
    public void Interact() { }
    private void Awake() { if (playerCamera == null) playerCamera = Camera.main; }
    private void Update()
    {
        if (Input.GetKeyDown(OptionsManager.Interact)) TryInteract();
    }
    private void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                if (fuseObject != null && fuseObject.activeSelf && bridgeController != null)
                    bridgeController.ActivateBridge();
    }
}