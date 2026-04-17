using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string sceneName;

    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    public int currentHealth;
    public int shotgunAmmo;
    public int rifleAmmo;

    public List<SavedInventoryItem> inventoryItems = new List<SavedInventoryItem>();
}

[Serializable]
public class SavedInventoryItem
{
    public string itemName;
    public string description;
}