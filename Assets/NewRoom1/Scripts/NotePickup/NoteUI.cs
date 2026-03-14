using UnityEngine;
using TMPro;

public class NoteUI : MonoBehaviour
{
    [SerializeField] private GameObject notePanel;
    [SerializeField] private TextMeshProUGUI noteTextDisplay;
    
    private bool isOpen;
    
    void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.E))
        {
            CloseNote();
        }
    }
    
    public void ShowNote(string text)
    {
        noteTextDisplay.text = text;
        notePanel.SetActive(true);
        isOpen = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // Pause game while reading
    }
    
    public void CloseNote()
    {
        notePanel.SetActive(false);
        isOpen = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}
