using UnityEngine;
public class BridgeButton : MonoBehaviour, IInteractable
{
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public LayerMask interactLayer = ~0;
    public BridgeController bridgeController;
    public bool oneUseOnly = true;
    private bool hasBeenUsed = false;
    public string PromptText => hasBeenUsed ? "Already activated" : "Activate bridge";
    public void Interact() { }
    private void Awake() { if (playerCamera == null) playerCamera = Camera.main; }
    private void Update()
    {
        if (oneUseOnly && hasBeenUsed) return;
        if (Input.GetKeyDown(OptionsManager.Interact)) TryInteract();
    }
    private void TryInteract()
    {
        if (playerCamera == null) return;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                PressButton();
    }
    private void PressButton()
    {
        if (bridgeController == null) return;
        hasBeenUsed = true;
        bridgeController.ActivateBridge();
    }
}