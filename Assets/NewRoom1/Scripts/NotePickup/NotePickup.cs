using UnityEngine;
using TMPro;

public class NotePickup : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public string promptText = "Read note";
    public string PromptText => promptText;

    [Header("Note Content")]
    [TextArea(5, 10)]
    public string noteContent;

    [Header("UI")]
    public GameObject notePanel;
    public TMP_Text noteText;

    [Header("Inventory")]
    public ItemData noteItemData; // assign in Inspector like KeyPickup2

    public static bool IsOpen { get; private set; } = false;
    private static NotePickup currentNote;

    void Awake()
    {
        IsOpen = false;
    }

    public void Interact()
    {
        if (IsOpen) return;
        OpenNote();
    }

    void OpenNote()
    {
        Debug.Log("OpenNote called");
        notePanel.SetActive(true);
        notePanel.transform.SetAsLastSibling();
        noteText.text = noteContent;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        IsOpen = true;
        currentNote = this;
    }

    public void CloseNote()
    {
        Debug.Log("CloseNote called");
        notePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        IsOpen = false;

        if (noteItemData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.TryAddItem(noteItemData, 1);
            PickupNotification.Show(noteItemData.icon, noteItemData.itemName, 1);
        }

        Destroy(currentNote.gameObject);
        currentNote = null;
    }
}