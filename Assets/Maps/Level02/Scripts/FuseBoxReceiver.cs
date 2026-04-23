using UnityEngine;
public class FuseBoxReceiver : MonoBehaviour, IInteractable
{
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public LayerMask interactLayer = ~0;
    public string requiredItemName = "Fuse";
    public GameObject insertedFuseVisual;
    public BridgeController bridgeController;
    public bool fuseInserted = false;
    public string PromptText => (insertedFuseVisual != null && !insertedFuseVisual.activeSelf) ? "Insert Fuse" : "";
    public void Interact() { }
    private void Awake() { if (playerCamera == null) playerCamera = Camera.main; if (insertedFuseVisual != null) insertedFuseVisual.SetActive(false); }
    private void Update()
    {
        if (fuseInserted) return;
        if (Input.GetKeyDown(OptionsManager.Interact))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
                if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                    TryInsertFuse();
        }
    }
    private void TryInsertFuse()
    {
        if (InventoryManager.Instance == null) return;
        if (!InventoryManager.Instance.RemoveItemByName(requiredItemName, 1)) return;
        fuseInserted = true;
        if (insertedFuseVisual != null) insertedFuseVisual.SetActive(true);
        if (bridgeController != null) bridgeController.ActivateBridge();
    }
}