using UnityEngine;
public class FuseBox : MonoBehaviour, IInteractable
{
    public enum FuseBoxType { SourceBox, TargetBox }
    public FuseBoxType boxType;
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public LayerMask interactLayer = ~0;
    public ItemData fuseItemData;
    public string requiredItemName = "Fuse";
    public GameObject fuseVisual;
    public BridgeController connectedBridge;
    public bool hasFuseInside = true;
    public bool alreadyUsed = false;
    public string PromptText => boxType == FuseBoxType.SourceBox ? (hasFuseInside ? "Take fuse" : "Empty") : (hasFuseInside ? "Fuse already inserted" : "Insert fuse");
    public void Interact() { }
    private void Awake() { if (playerCamera == null) playerCamera = Camera.main; if (fuseVisual != null) fuseVisual.SetActive(hasFuseInside); }
    private void Update()
    {
        if (Input.GetKeyDown(OptionsManager.Interact)) TryInteract();
    }
    private void TryInteract()
    {
        if (playerCamera == null) return;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            { if (boxType == FuseBoxType.SourceBox) TakeFuse(); else InsertFuse(); }
    }
    private void TakeFuse()
    {
        if (alreadyUsed || !hasFuseInside || InventoryManager.Instance == null || fuseItemData == null) return;
        if (!InventoryManager.Instance.TryAddItem(fuseItemData, 1, true)) return;
        hasFuseInside = false; alreadyUsed = true;
        if (fuseVisual != null) fuseVisual.SetActive(false);
    }
    private void InsertFuse()
    {
        if (hasFuseInside || InventoryManager.Instance == null) return;
        if (!InventoryManager.Instance.RemoveItemByName(requiredItemName, 1)) return;
        hasFuseInside = true; alreadyUsed = true;
        if (fuseVisual != null) fuseVisual.SetActive(true);
        if (connectedBridge != null) connectedBridge.ActivateBridge();
    }
}