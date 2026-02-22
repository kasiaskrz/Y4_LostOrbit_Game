using UnityEngine;
using TMPro;

public class NoteUI : MonoBehaviour
{
    public GameObject notePanel;   
    public TMP_Text noteText;      

    private bool isOpen = false;

    void Start()
    {
        if (notePanel != null)
            notePanel.SetActive(false);

        isOpen = false;
    }

    public void Toggle(string text)
    {
        isOpen = !isOpen;

        if (notePanel != null)
            notePanel.SetActive(isOpen);

        if (isOpen)
        {
            if (noteText != null)
                noteText.text = text;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            if (noteText != null)
                noteText.text = "";   // clear text when closing

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }
}
