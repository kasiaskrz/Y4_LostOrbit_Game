using UnityEngine;

public class FuseBoxReceiver : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public LayerMask interactLayer = ~0;

    [Header("Requirement")]
    public string requiredItemName = "Fuse";

    [Header("Visuals / Result")]
    public GameObject insertedFuseVisual;
    public BridgeController bridgeController;

    [Header("Fuse Toggle (Visual/Audio)")]
    public FuseToggle fuseToggle;

    [Header("Locking")]
    public bool interactionEnabled = true;
    private bool hasBeenUnlocked = false;

    [Header("State")]
    public bool fuseInserted = false;

    public string PromptText
    {
        get
        {
            if (!interactionEnabled) return "";
            if (fuseInserted) return "";
            return "Insert Fuse";
        }
    }

    public void Interact() { }

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (insertedFuseVisual != null)
            insertedFuseVisual.SetActive(false);

        if (fuseToggle != null)
        {
            fuseToggle.enabled = interactionEnabled;

            if (insertedFuseVisual != null && insertedFuseVisual.activeSelf)
                fuseToggle.SetVisualState(true);
            else
                fuseToggle.SetVisualState(false);
        }

        if (interactionEnabled)
            hasBeenUnlocked = true;
    }

    private void Update()
    {
        if (!interactionEnabled) return;
        if (fuseInserted) return;

        if (Input.GetKeyDown(OptionsManager.Interact))
        {
            if (playerCamera == null) return;

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            {
                if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                {
                    TryInsertFuse();
                }
            }
        }
    }

    private void TryInsertFuse()
    {
        if (InventoryManager.Instance == null) return;
        if (!InventoryManager.Instance.RemoveItemByName(requiredItemName, 1)) return;

        fuseInserted = true;

        if (insertedFuseVisual != null)
            insertedFuseVisual.SetActive(true);

        if (fuseToggle != null)
            fuseToggle.ForcePlace();

        if (bridgeController != null)
            bridgeController.ActivateBridge();
    }

    public void SetInteractionEnabled(bool enabled)
    {
        if (hasBeenUnlocked && !enabled)
            return;

        interactionEnabled = enabled;

        if (enabled)
            hasBeenUnlocked = true;

        if (fuseToggle != null)
            fuseToggle.enabled = interactionEnabled;
    }

    public void UnlockReceiver()
    {
        interactionEnabled = true;
        hasBeenUnlocked = true;

        if (fuseToggle != null)
            fuseToggle.enabled = true;
    }

    public void LockReceiver()
    {
        if (hasBeenUnlocked)
            return;

        interactionEnabled = false;

        if (fuseToggle != null)
            fuseToggle.enabled = false;
    }
}