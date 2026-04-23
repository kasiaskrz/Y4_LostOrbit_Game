using UnityEngine;

public class FuseBox : MonoBehaviour, IInteractable
{
    public enum FuseBoxType
    {
        SourceBox,
        TargetBox
    }

    [Header("Box Type")]
    public FuseBoxType boxType;

    [Header("Interaction")]
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public LayerMask interactLayer = ~0;
    public bool interactionEnabled = true;

    [Header("Fuse Data")]
    public ItemData fuseItemData;
    public string requiredItemName = "Fuse";
    public GameObject fuseVisual;

    [Header("Fuse Toggle (Visual/Audio)")]
    public FuseToggle fuseToggle;

    [Header("Bridge")]
    public BridgeController connectedBridge;

    [Header("Unlock Other Box")]
    public FuseBoxReceiver receiverToUnlock;
    public bool unlockReceiverOnTakeFuse = true;
    private bool hasUnlockedReceiverPermanently = false;

    [Header("State")]
    public bool hasFuseInside = true;
    public bool alreadyUsed = false;

    public string PromptText
    {
        get
        {
            if (!interactionEnabled) return "";

            if (boxType == FuseBoxType.SourceBox)
                return hasFuseInside ? "Take fuse" : "Empty";

            return hasFuseInside ? "Fuse already inserted" : "Insert fuse";
        }
    }

    public void Interact() { }

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (fuseVisual != null)
            fuseVisual.SetActive(hasFuseInside);

        if (fuseToggle != null)
        {
            if (hasFuseInside)
                fuseToggle.SetVisualState(true);
            else
                fuseToggle.SetVisualState(false);
        }
    }

    private void Update()
    {
        if (!interactionEnabled) return;

        if (Input.GetKeyDown(OptionsManager.Interact))
            TryInteract();
    }

    private void TryInteract()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                if (boxType == FuseBoxType.SourceBox)
                    TakeFuse();
                else
                    InsertFuse();
            }
        }
    }

    private void TakeFuse()
    {
        if (alreadyUsed) return;
        if (!hasFuseInside) return;
        if (InventoryManager.Instance == null) return;
        if (fuseItemData == null) return;

        if (!InventoryManager.Instance.TryAddItem(fuseItemData, 1, true))
            return;

        hasFuseInside = false;
        alreadyUsed = true;

        if (fuseVisual != null)
            fuseVisual.SetActive(false);

        if (fuseToggle != null)
            fuseToggle.ForceTake();

        if (unlockReceiverOnTakeFuse && receiverToUnlock != null && !hasUnlockedReceiverPermanently)
        {
            receiverToUnlock.UnlockReceiver();
            hasUnlockedReceiverPermanently = true;
        }
    }

    private void InsertFuse()
    {
        if (alreadyUsed) return;
        if (hasFuseInside) return;
        if (InventoryManager.Instance == null) return;

        if (!InventoryManager.Instance.RemoveItemByName(requiredItemName, 1))
            return;

        hasFuseInside = true;
        alreadyUsed = true;

        if (fuseVisual != null)
            fuseVisual.SetActive(true);

        if (fuseToggle != null)
            fuseToggle.ForcePlace();

        if (connectedBridge != null)
            connectedBridge.ActivateBridge();
    }

    public void SetInteractionEnabled(bool enabled)
    {
        interactionEnabled = enabled;
    }
}