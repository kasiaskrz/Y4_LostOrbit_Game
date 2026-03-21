using UnityEngine;

// Right-click in Project > Create > Lost Orbit > Item to make new items
[CreateAssetMenu(fileName = "NewItem", menuName = "Lost Orbit/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemName = "New Item";
    [TextArea] public string description = "";
    public Sprite icon;

    [Header("Type")]
    public ItemType itemType;

    [Header("Stack Settings")]
    public bool stackable = true;
    public int maxStack = 99;

    [Header("Drop Settings")]
    public GameObject worldPrefab; // The prefab dropped/spawned in the world

    [Header("Note Content")]
    [TextArea(5, 10)]
    public string noteContent; // only used when itemType == Note
    
}

public enum ItemType
{
    Key,
    Ammo,
    Note,
    Fuse,
    PowerCell
}
