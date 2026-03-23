using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Interactor : MonoBehaviour
{
    [Header("Raycast")]
    public Camera cam;
    public float interactDistance = 3.5f;
    public LayerMask interactMask = ~0;
    public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

    [Header("UI")]
    public Image crosshair;
    public Color idleColor = Color.white;
    public Color highlightColor = Color.green;

    [Header("Interaction Prompt")]
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;

    public static IInteractable CurrentInteractable { get; private set; }

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    void Update()
    {
        if (Time.timeScale == 0f && !NotePickup.IsOpen && !WirePuzzle.IsOpen) return;

        FindInteractable();
        UpdateCrosshair();
        UpdatePrompt();

        if (Input.GetKeyDown(KeyCode.E) && !NotePickup.IsOpen && !WirePuzzle.IsOpen && CurrentInteractable != null)
            CurrentInteractable.Interact();

        if (Input.GetKeyDown(KeyCode.Escape) && NotePickup.IsOpen)
        {
            NotePickup note = FindFirstObjectByType<NotePickup>();
            if (note != null)
                note.CloseNote();
            else
                NoteReader.Instance.CloseNote();
        }

        // close wire puzzle with Escape
        if (Input.GetKeyDown(KeyCode.Escape) && WirePuzzle.IsOpen)
        {
            WirePuzzle puzzle = FindFirstObjectByType<WirePuzzle>();
            if (puzzle != null)
                puzzle.puzzlePanel.SetActive(false);
        }
    }

    void FindInteractable()
    {
        CurrentInteractable = null;
        if (cam == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask, triggerInteraction))
            CurrentInteractable = hit.collider.GetComponent<IInteractable>();
    }

    void UpdateCrosshair()
    {
        if (crosshair == null) return;
        crosshair.color = (CurrentInteractable != null) ? highlightColor : idleColor;
    }

    void UpdatePrompt()
    {
        if (promptPanel == null) return;

        if (CurrentInteractable != null && !NotePickup.IsOpen && !WirePuzzle.IsOpen)
        {
            promptPanel.SetActive(true);
            if (promptText != null)
                promptText.text = $"[E] {CurrentInteractable.PromptText}";
        }
        else
        {
            promptPanel.SetActive(false);
        }
    }
}