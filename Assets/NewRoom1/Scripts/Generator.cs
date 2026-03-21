using UnityEngine;

public class Generator : MonoBehaviour, IInteractable
{
    [Header("Required Item")]
    public ItemData powerCellData;

    public bool isPowered = false;

    [Header("Visual")]
    public GameObject powerCellPrefab;
    public Transform slotPoint;

    [Header("Emission")]
    public Renderer emissionRenderer;  // drag the child mesh here
    public Color poweredColor = Color.green;

    private GameObject currentCell;
    private Material emissionMat;

    public string PromptText
    {
        get
        {
            if (isPowered) return "";
            return InventoryManager.Instance != null &&
                   InventoryManager.Instance.HasItem(powerCellData)
                ? "Insert Power Cell"
                : "Requires Power Cell";
        }
    }

    void Awake()
    {
        if (emissionRenderer != null)
        {
            Material[] mats = emissionRenderer.materials; // get all instanced materials
            if (mats.Length > 1)
                emissionMat = mats[1]; // index 1 = second material
        }
    }

    public void Interact()
    {
        if (isPowered) return;

        if (InventoryManager.Instance != null &&
            InventoryManager.Instance.HasItem(powerCellData))
            InsertPowerCell();
        else
            Debug.Log("No power cell in inventory");
    }

    void InsertPowerCell()
    {
        bool removed = InventoryManager.Instance.TryRemoveItem(powerCellData, 1);
        if (!removed) { Debug.LogWarning("Failed to remove power cell"); return; }

        SpawnPowerCell();
        SetEmissionColor(poweredColor);

        isPowered = true;
        Debug.Log("Generator powered!");
    }

    void SpawnPowerCell()
    {
        if (powerCellPrefab == null || slotPoint == null) return;

        currentCell = Instantiate(powerCellPrefab, slotPoint.position, slotPoint.rotation);
        currentCell.transform.SetParent(slotPoint);
        currentCell.transform.localPosition = Vector3.zero;
        currentCell.transform.localRotation = Quaternion.identity;
    }

    void SetEmissionColor(Color color)
    {
        if (emissionMat == null) return;

        emissionMat.EnableKeyword("_EMISSION");
        emissionMat.SetColor("_EmissionColor", color);
        DynamicGI.UpdateEnvironment();
    }
}