using UnityEngine;

public class FuseBoxReceiver : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactLayer = ~0;

    [Header("Fuse")]
    public string requiredItemName = "Fuse";
    public GameObject insertedFuseVisual;

    [Header("Bridge")]
    public BridgeController bridgeController;

    [Header("State")]
    public bool fuseInserted = false;

    public string PromptText => fuseInserted ? "Fuse already inserted" : "Insert fuse";
    public void Interact() { }

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (insertedFuseVisual != null)
            insertedFuseVisual.SetActive(false);
    }

    private void Update()
    {
        if (fuseInserted) return;

        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            {
                if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                    TryInsertFuse();
            }
        }
    }

    private void TryInsertFuse()
    {
        if (InventoryManager.Instance == null) { Debug.LogWarning("InventoryManager not found."); return; }

        bool removed = InventoryManager.Instance.RemoveItemByName(requiredItemName, 1);
        if (!removed) { Debug.Log("You need a fuse."); return; }

        fuseInserted = true;
        if (insertedFuseVisual != null) insertedFuseVisual.SetActive(true);
        Debug.Log("Fuse inserted.");
        if (bridgeController != null) bridgeController.ActivateBridge();
    }
}