using UnityEngine;

public class FuseBox : MonoBehaviour
{
    public enum FuseBoxType
    {
        SourceBox,
        TargetBox
    }

    [Header("Fuse Box Settings")]
    public FuseBoxType boxType;

    [Header("Interaction")]
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactLayer = ~0;

    [Header("Fuse Item")]
    public ItemData fuseItemData;
    public string requiredItemName = "Fuse";

    [Header("Fuse Visual")]
    public GameObject fuseVisual;

    [Header("Target Settings")]
    public BridgeController connectedBridge;

    [Header("State")]
    public bool hasFuseInside = true;
    public bool alreadyUsed = false;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (fuseVisual != null)
            fuseVisual.SetActive(hasFuseInside);
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
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                if (boxType == FuseBoxType.SourceBox)
                {
                    TakeFuse();
                }
                else if (boxType == FuseBoxType.TargetBox)
                {
                    InsertFuse();
                }
            }
        }
    }

    private void TakeFuse()
    {
        if (alreadyUsed || !hasFuseInside)
        {
            Debug.Log("This fuse box is empty.");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager not found.");
            return;
        }

        if (fuseItemData == null)
        {
            Debug.LogWarning("Fuse ItemData is missing.");
            return;
        }

        bool added = InventoryManager.Instance.TryAddItem(fuseItemData, 1, true);

        if (!added)
        {
            Debug.Log("Inventory full.");
            return;
        }

        hasFuseInside = false;
        alreadyUsed = true;

        if (fuseVisual != null)
            fuseVisual.SetActive(false);

        Debug.Log("Fuse taken.");
    }

    private void InsertFuse()
    {
        if (hasFuseInside)
        {
            Debug.Log("Fuse already inserted here.");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager not found.");
            return;
        }

        bool removed = InventoryManager.Instance.RemoveItemByName(requiredItemName, 1);

        if (!removed)
        {
            Debug.Log("You need a fuse.");
            return;
        }

        hasFuseInside = true;
        alreadyUsed = true;

        if (fuseVisual != null)
            fuseVisual.SetActive(true);

        Debug.Log("Fuse inserted.");

        if (connectedBridge != null)
        {
            connectedBridge.ActivateBridge();
        }
    }
}