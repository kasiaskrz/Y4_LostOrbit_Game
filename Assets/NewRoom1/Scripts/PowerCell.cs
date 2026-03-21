using UnityEngine;

public class PowerCellPickup : MonoBehaviour, IInteractable
{
    [Header("Inventory")]
    public ItemData powerCellData;

    private Collider col;

    public string PromptText => "Pick Up Power Cell";

    void Awake()
    {
        col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false; // disable interaction at start
    }

    public void Interact()
    {
        Debug.Log("[PowerCell] Collected!");

        if (powerCellData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.TryAddItem(powerCellData, 1);
            PickupNotification.Show(powerCellData.icon, powerCellData.itemName, 1);
        }

        gameObject.SetActive(false);
    }
}