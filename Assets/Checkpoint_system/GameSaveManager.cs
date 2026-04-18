using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;

    public SaveData currentSave = new SaveData();
    public bool hasCheckpoint = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveCheckpoint(Transform playerTransform, PlayerHealth playerHealth, PlayerAmmo playerAmmo, Inventory inventory)
    {
        if (playerTransform == null) return;

        currentSave.sceneName = SceneManager.GetActiveScene().name;

        currentSave.playerPosX = playerTransform.position.x;
        currentSave.playerPosY = playerTransform.position.y;
        currentSave.playerPosZ = playerTransform.position.z;

        if (playerHealth != null)
        {
            currentSave.currentHealth = playerHealth.currentHealth;
        }

        if (playerAmmo != null)
        {
            currentSave.shotgunAmmo = playerAmmo.shotgunAmmo;
            currentSave.rifleAmmo = playerAmmo.rifleAmmo;
        }

        SaveInventoryFromExistingScript(inventory);

        hasCheckpoint = true;
        Debug.Log("Checkpoint saved.");
    }

    private void SaveInventoryFromExistingScript(Inventory inventory)
    {
        currentSave.inventoryItems.Clear();

        if (inventory == null)
        {
            Debug.LogWarning("No Inventory found while saving.");
            return;
        }

        FieldInfo itemsField = typeof(Inventory).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance);

        if (itemsField == null)
        {
            Debug.LogWarning("Could not find private 'items' field on Inventory.");
            return;
        }

        List<Item> items = itemsField.GetValue(inventory) as List<Item>;

        if (items == null) return;

        foreach (Item item in items)
        {
            if (item == null) continue;

            currentSave.inventoryItems.Add(new SavedInventoryItem
            {
                itemName = item.name,
                description = item.description
            });
        }
    }

    public void LoadInventoryIntoExistingScript(Inventory inventory)
    {
        if (inventory == null) return;

        FieldInfo itemsField = typeof(Inventory).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance);

        if (itemsField == null)
        {
            Debug.LogWarning("Could not find private 'items' field on Inventory.");
            return;
        }

        List<Item> rebuiltItems = new List<Item>();

        foreach (SavedInventoryItem savedItem in currentSave.inventoryItems)
        {
            rebuiltItems.Add(new Item(savedItem.itemName, savedItem.description));
        }

        itemsField.SetValue(inventory, rebuiltItems);

        MethodInfo updateMethod = typeof(Inventory).GetMethod("UpdateInventoryDisplay", BindingFlags.NonPublic | BindingFlags.Instance);
        if (updateMethod != null)
        {
            updateMethod.Invoke(inventory, null);
        }
    }

    public Vector3 GetSavedPosition()
    {
        return new Vector3(
            currentSave.playerPosX,
            currentSave.playerPosY,
            currentSave.playerPosZ
        );
    }

    public void RespawnPlayer(GameObject player, PlayerHealth playerHealth, PlayerAmmo playerAmmo, Inventory inventory)
    {
        if (!hasCheckpoint || player == null) return;

        player.transform.position = GetSavedPosition();

        if (playerHealth != null)
        {
            playerHealth.currentHealth = currentSave.currentHealth;
            playerHealth.OnHealthChanged?.Invoke(playerHealth.currentHealth, playerHealth.maxHealth);
        }

        if (playerAmmo != null)
        {
            playerAmmo.shotgunAmmo = currentSave.shotgunAmmo;
            playerAmmo.rifleAmmo = currentSave.rifleAmmo;
        }

        LoadInventoryIntoExistingScript(inventory);

        hasCheckpoint = false; // after one respawn, next death goes to LoseScene
        Debug.Log("Respawned from latest checkpoint.");
    }
}