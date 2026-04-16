using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Simple linear tutorial for SC001.
/// Each step shows instruction text and waits for the player to do the action.
/// No zone triggers needed — just one script on a GameObject in SC001.
///
/// Setup:
///   1. Attach to an empty GameObject in SC001
///   2. Assign tutorialText (TMP) and tutorialCanvasGroup (CanvasGroup on HintPanel)
///   3. Assign playerBody (the Player GameObject with CharacterController)
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI tutorialText;
    public CanvasGroup tutorialCanvasGroup;

    [Header("Player Reference")]
    [Tooltip("Drag the Player GameObject here.")]
    public GameObject playerBody;

    [Header("Fade Settings")]
    public float fadeSpeed = 3f;

    // Internal state
    private int currentStep = 0;
    private bool stepComplete = false;
    private bool tutorialDone = false;

    private CharacterController cc;
    private bool startPositionSet = false;
    private Vector3 startPosition;

    private Coroutine displayCoroutine;

    // ── Unity ────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (tutorialCanvasGroup != null)
            tutorialCanvasGroup.alpha = 0f;

        if (playerBody != null)
        {
            cc = playerBody.GetComponent<CharacterController>();
            // startPosition set on first update
        }

        StartCoroutine(RunTutorial());
    }

    // ── Tutorial Flow ────────────────────────────────────────────────────────

    private IEnumerator RunTutorial()
    {
        // Step 1 — Movement
        yield return ShowText("Use [W][A][S][D] to move around.");
        yield return WaitForCondition(PlayerHasMoved, "Use [W][A][S][D] to move around.");

        // Step 2 — Sprint
        yield return CrossfadeTo("Good!\nNow hold [⇧ Shift] to sprint.");
        yield return WaitForCondition(PlayerIsSprinting, "Hold [⇧ Shift] to sprint.");

        // Step 3 — Jump
        yield return CrossfadeTo("Nice!\nPress [␣ Space] to jump.");
        yield return WaitForCondition(PlayerJumped, "Press [␣ Space] to jump.");

        // Step 4 — Interact
        yield return CrossfadeTo("Great!\nWalk up to the box and press [E] to push it.");
        yield return WaitForCondition(PlayerPressedInteract, "Walk up to the box and press [E] to push it.");

        // Step 5 — Inventory
        yield return CrossfadeTo("Press [TAB] to open your inventory.\nYou can click and drag items to move them.");
        yield return WaitForCondition(PlayerOpenedInventory, "Press [TAB] to open your inventory.");

        // Step 6 — Close Inventory
        yield return CrossfadeTo("Now press [TAB] again to close your inventory.");
        yield return WaitForCondition(PlayerClosedInventory, "Press [TAB] again to close your inventory.");

        // Step 7 — Pause
        yield return CrossfadeTo("Press [ESC] to open the pause menu.\nFind the Help panel for all controls.");
        yield return WaitForCondition(PlayerOpenedPause, "Press [ESC] to open the pause menu.");

        // Step 8 — Close Pause
        yield return CrossfadeTo("Press [ESC] again to close the pause menu.");
        yield return WaitForCondition(PlayerClosedPause, "Press [ESC] again to close the pause menu.");

        // Done
        yield return CrossfadeTo("Training complete!\nHead to the exit to enter the Main Hall.");
        yield return new WaitForSeconds(4f);
        yield return FadeTo(0f);

        tutorialDone = true;
    }

    // ── Condition Checks ─────────────────────────────────────────────────────

    private bool PlayerHasMoved()
    {
        if (playerBody == null) return false;

        // Set start position on first check so we get the real spawn position
        if (!startPositionSet)
        {
            startPosition = playerBody.transform.position;
            startPositionSet = true;
            return false;
        }

        return Vector3.Distance(playerBody.transform.position, startPosition) > 1.5f;
    }

    private bool PlayerIsSprinting()
    {
        return Input.GetKey(KeyCode.LeftShift) &&
               (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));
    }

    private bool PlayerJumped()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private bool PlayerPressedInteract()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    private bool inventoryWasOpen = false;
    private bool PlayerOpenedInventory()
    {
        bool isOpen = Time.timeScale == 0f;
        bool justOpened = isOpen && !inventoryWasOpen;
        inventoryWasOpen = isOpen;
        return justOpened;
    }

    private bool PlayerClosedInventory()
    {
        bool isOpen = Time.timeScale == 0f;
        bool justClosed = !isOpen && inventoryWasOpen;
        inventoryWasOpen = isOpen;
        return justClosed;
    }

    private bool pauseWasOpen = false;
    private bool PlayerOpenedPause()
    {
        // Pause menu sets timeScale to 0 — but so does inventory
        // We detect ESC keydown specifically here
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseWasOpen = true;
            return true;
        }
        return false;
    }

    private bool PlayerClosedPause()
    {
        if (pauseWasOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            pauseWasOpen = false;
            return true;
        }
        return false;
    }

    // ── Wait Helper ──────────────────────────────────────────────────────────

    /// <summary>Waits until condition is true. Repeating idle hint every 10s.</summary>
    private IEnumerator WaitForCondition(System.Func<bool> condition, string reminder)
    {
        float idleTimer = 0f;

        while (!condition())
        {
            // Count up idle time only when game is running
            if (Time.timeScale > 0f)
                idleTimer += Time.deltaTime;

            // Show reminder after 10 seconds of inactivity
            if (idleTimer >= 10f)
            {
                idleTimer = 0f;
                if (displayCoroutine != null) StopCoroutine(displayCoroutine);
                displayCoroutine = StartCoroutine(FlashReminder(reminder));
            }

            yield return null;
        }
    }

    // ── Display ──────────────────────────────────────────────────────────────

    private IEnumerator ShowText(string message)
    {
        if (tutorialText != null) tutorialText.text = message;
        yield return FadeTo(1f);
    }

    private IEnumerator CrossfadeTo(string message)
    {
        yield return FadeTo(0f);
        yield return new WaitForSeconds(0.2f);
        if (tutorialText != null) tutorialText.text = message;
        yield return FadeTo(1f);
    }

    private IEnumerator FlashReminder(string message)
    {
        // Briefly pulse the text to remind player
        yield return FadeTo(0f);
        if (tutorialText != null) tutorialText.text = message;
        yield return FadeTo(1f);
    }

    private IEnumerator FadeTo(float target)
    {
        if (tutorialCanvasGroup == null) yield break;

        while (Mathf.Abs(tutorialCanvasGroup.alpha - target) > 0.01f)
        {
            tutorialCanvasGroup.alpha = Mathf.MoveTowards(
                tutorialCanvasGroup.alpha, target, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        tutorialCanvasGroup.alpha = target;
    }
}