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

    [Header("Boss Targeting")]
    public Color bossAimColor = Color.red;

    public static IInteractable CurrentInteractable { get; private set; }

    private bool aimedAtBoss = false;
    private float bossDetectRange = 60f;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (promptPanel != null) promptPanel.SetActive(false);
        ShotgunShooter shooter = FindFirstObjectByType<ShotgunShooter>();
        if (shooter != null) bossDetectRange = shooter.range;
    }

    void Update()
    {
        if (Time.timeScale == 0f && !NotePickup.IsOpen && !WirePuzzle.IsOpen) return;

        FindInteractable();
        CheckBossAim();
        UpdateCrosshair();
        UpdatePrompt();

        if (Input.GetKeyDown(OptionsManager.Interact) && !NotePickup.IsOpen && !WirePuzzle.IsOpen && CurrentInteractable != null)
            CurrentInteractable.Interact();

        if (Input.GetKeyDown(OptionsManager.Pause) && NotePickup.IsOpen)
        {
            NotePickup note = FindFirstObjectByType<NotePickup>();
            if (note != null) note.CloseNote();
            else NoteReader.Instance.CloseNote();
        }

        if (Input.GetKeyDown(OptionsManager.Pause) && WirePuzzle.IsOpen)
        {
            WirePuzzle puzzle = FindFirstObjectByType<WirePuzzle>();
            if (puzzle != null) puzzle.puzzlePanel.SetActive(false);
        }
    }

    void FindInteractable()
    {
        CurrentInteractable = null;
        if (cam == null) return;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask, triggerInteraction))
            CurrentInteractable = hit.collider.GetComponentInParent<IInteractable>();
    }

    void CheckBossAim()
    {
        if (cam == null) { aimedAtBoss = false; return; }
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, bossDetectRange))
            aimedAtBoss = hit.collider.CompareTag("Boss");
        else
            aimedAtBoss = false;
    }

    void UpdateCrosshair()
    {
        if (crosshair == null) return;
        if (aimedAtBoss) crosshair.color = bossAimColor;
        else if (CurrentInteractable != null) crosshair.color = highlightColor;
        else crosshair.color = idleColor;
    }

    void UpdatePrompt()
    {
        if (promptPanel == null) return;
        if (CurrentInteractable != null && !NotePickup.IsOpen && !WirePuzzle.IsOpen)
        {
            promptPanel.SetActive(true);
            if (promptText != null)
                promptText.text = (CurrentInteractable is LaserEmitterInteractable)
                    ? CurrentInteractable.PromptText
                    : $"[{OptionsManager.Interact}] {CurrentInteractable.PromptText}";
        }
        else
        {
            promptPanel.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (cam == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(cam.transform.position, cam.transform.forward * interactDistance);
    }
}