using UnityEngine;
using TMPro;

public class NoteReader : MonoBehaviour
{

    [Header("Overlay")]
    public GameObject noteOverlay;
    public static NoteReader Instance;

    [Header("UI")]
    public GameObject notePanel;
    public TMP_Text noteText;

    void Awake()
    {
        Instance = this;
    }

    public void OpenNote(ItemData itemData)
    {
        notePanel.SetActive(true);
        notePanel.transform.SetAsLastSibling();
        noteText.text = itemData.noteContent;

        NotePickup.IsOpen = true;
        if (noteOverlay != null) noteOverlay.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CloseNote()
    {
        notePanel.SetActive(false);
        NotePickup.IsOpen = false;
        if (noteOverlay != null) noteOverlay.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}