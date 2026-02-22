using UnityEngine;

public class NotePickup : MonoBehaviour, IInteractable
{
    [TextArea(3, 10)]
    public string noteContent;

    [Header("Assign your Canvas here (the one that has NoteUI on it)")]
    public GameObject noteCanvasObject;

    [Header("Auto-disable controller while reading")]
    public string controllerTypeName = "FPS_PlayerMovement";

    NoteUI noteUI;
    MonoBehaviour controller;

    bool reading;
    public static bool IsReading;

    void Awake()
    {
        // Find NoteUI on the canvas object
        if (noteCanvasObject != null)
            noteUI = noteCanvasObject.GetComponent<NoteUI>();

        // Find player controller by tag
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            foreach (var mb in player.GetComponents<MonoBehaviour>())
            {
                if (mb != null && mb.GetType().Name == controllerTypeName)
                {
                    controller = mb;
                    break;
                }
            }
        }
    }

    public void Interact()
    {
        Debug.Log("NOTE Interact called on: " + gameObject.name);

        if (noteUI == null)
        {
            Debug.LogError("NotePickup: NoteUI not found. Assign noteCanvasObject with NoteUI on it.");
            return;
        }
        if (reading) return;

        reading = true;
        IsReading = true;

        noteUI.Toggle(noteContent);

        if (controller != null)
            controller.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!reading) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    void Close()
    {
        reading = false;
        IsReading = false;

        if (noteUI != null)
            noteUI.Toggle("");

        if (controller != null)
            controller.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
