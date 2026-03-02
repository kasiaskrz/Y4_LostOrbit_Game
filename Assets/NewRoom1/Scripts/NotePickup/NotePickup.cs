using UnityEngine;

public class NotePickup : MonoBehaviour, IInteractable
{
    [TextArea(3, 10)]
    public string noteContent;

    [Header("UI")]
    public GameObject noteCanvasObject;

    [Header("Auto-disable controller while reading")]
    public string controllerTypeName = "FPS_PlayerMovement";

    [Header("Close Keys")]
    public KeyCode primaryCloseKey = KeyCode.E;
    public KeyCode secondaryCloseKey = KeyCode.Escape;

    private NoteUI noteUI;
    private MonoBehaviour controller;
    private bool reading = false;

    public string PromptText => reading ? "Close Note" : "Read Note";

    void Awake()
    {
        if (noteCanvasObject != null)
            noteUI = noteCanvasObject.GetComponent<NoteUI>();

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
        if (noteUI == null) return;

        if (reading) { Close(); return; }

        reading = true;
        noteUI.Toggle(noteContent);

        if (controller != null)
            controller.enabled = false;
    }

    void Update()
    {
        if (!reading) return;

        if (Input.GetKeyDown(primaryCloseKey) || Input.GetKeyDown(secondaryCloseKey))
            Close();
    }

    void Close()
    {
        reading = false;

        if (noteUI != null)
            noteUI.Toggle("");

        if (controller != null)
            controller.enabled = true;
    }
}