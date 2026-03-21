using System;
using System.Collections.Generic; // This line is important
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private List<Item> items = new List<Item>();
    public GameObject inventoryPanel; // Assign this in the Inspector
    public Text inventoryText; // Assign this in the Inspector

    private void Start()
    {
        // Hide inventory at the start
        inventoryPanel.SetActive(false);
    }

    public void AddItem(Item item)
    {
        items.Add(item);
        Debug.Log(item.name + " has been added to the inventory.");
        UpdateInventoryDisplay();
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf)
        {
            UpdateInventoryDisplay();
        }
    }

    private void UpdateInventoryDisplay()
    {
        inventoryText.text = ""; // Clear current text
        foreach (var item in items)
        {
            inventoryText.text += item.name + "\n"; // Display each item
        }
    }

    internal void AddItem(ItemData powerCellData)
    {
        throw new NotImplementedException();
    }
}

// Simple Item class for inventory items
[System.Serializable]
public class Item
{
    public string name;
    public string description;

    public Item(string name, string description)
    {
        this.name = name;
        this.description = description;
    }
}