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
    [Header("Audio")]
    public AudioSource audioSource;
    [Header("Overlay")]
    public GameObject noteOverlay;
    [Header("Guidance")]
    public GameObject roomHintPopup; // assign RoomHintPopup panel in Inspector
    public static bool IsOpen { get; set; } = false;
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
        if (audioSource != null)
            audioSource.Play();
        notePanel.SetActive(true);
        notePanel.transform.SetAsLastSibling();
        noteText.text = noteItemData.noteContent;
        if (noteOverlay != null) noteOverlay.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // pause game
        IsOpen = true;
        currentNote = this;
    }

    public void CloseNote()
    {
        Debug.Log("CloseNote called");
        notePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f; // unpause game
        IsOpen = false;
        if (noteItemData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.TryAddItem(noteItemData, 1);
            PickupNotification.Show(noteItemData.icon, noteItemData.itemName, 1);
        }
        if (noteOverlay != null) noteOverlay.SetActive(false);
        // Hide room hint popup once note is picked up
        if (roomHintPopup != null) roomHintPopup.SetActive(false);
        Destroy(currentNote.gameObject);
        currentNote = null;
    }

    void Update()
    {
        if (IsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNote();
            PauseMenu.EscConsumed = true;
        }
    }
}