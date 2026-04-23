using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI tutorialText;
    public CanvasGroup tutorialCanvasGroup;

    [Header("Player Reference")]
    public GameObject playerBody;

    [Header("Fade Settings")]
    public float fadeSpeed = 3f;

    [Header("Typewriter")]
    public float typeSpeed = 0.04f;
    public float linePause = 0.3f;
    public AudioClip typingSound;
    public AudioSource audioSource;

    private bool startPositionSet = false;
    private Vector3 startPosition;
    private Coroutine displayCoroutine;
    private bool tutorialDone = false;
    private bool pauseWasOpen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (tutorialCanvasGroup != null)
            tutorialCanvasGroup.alpha = 0f;
        StartCoroutine(RunTutorial());
    }

    private string Key(KeyCode k) => k.ToString();

    private IEnumerator RunTutorial()
    {
        yield return new WaitForSeconds(1f);

        if (GameProgress.Instance != null && GameProgress.Instance.tutorialComplete)
        {
            yield return TypeLines(new string[] {
                "// TRAINING RECORD FOUND",
                "All training protocols previously completed.",
                "Proceed to the Main Hall."
            });
            yield return new WaitForSeconds(3f);
            yield return FadeTo(0f);
            yield break;
        }

        // MOVEMENT
        yield return TypeLines(new string[] {
            "// SYSTEM BOOT COMPLETE",
            "Initialising movement protocols.",
            $"Use [{Key(OptionsManager.MoveForward)}][{Key(OptionsManager.MoveLeft)}][{Key(OptionsManager.MoveBack)}][{Key(OptionsManager.MoveRight)}] to navigate."
        });
        yield return WaitForCondition(PlayerHasMoved, new string[] {
            "// AWAITING INPUT",
            $"Move using [{Key(OptionsManager.MoveForward)}][{Key(OptionsManager.MoveLeft)}][{Key(OptionsManager.MoveBack)}][{Key(OptionsManager.MoveRight)}]."
        });

        // SPRINT
        yield return TypeLines(new string[] {
            "// MOBILITY UPGRADE AVAILABLE",
            $"Hold [{Key(OptionsManager.Sprint)}] to engage sprint mode."
        });
        yield return WaitForCondition(PlayerIsSprinting, new string[] {
            "// AWAITING INPUT",
            $"Hold [{Key(OptionsManager.Sprint)}] while moving to sprint."
        });

        // JUMP
        yield return TypeLines(new string[] {
            "// JUMP PROTOCOL ACTIVE",
            $"Press [{Key(OptionsManager.Jump)}] to engage jump thrusters."
        });
        yield return WaitForCondition(PlayerJumped, new string[] {
            "// AWAITING INPUT",
            $"Press [{Key(OptionsManager.Jump)}] to jump."
        });

        // INTERACT
        yield return TypeLines(new string[] {
            "// INTERACTION MODULE ONLINE",
            "Approach the container unit.",
            $"Press [{Key(OptionsManager.Interact)}] to interact with objects."
        });
        yield return WaitForCondition(PlayerPressedInteract, new string[] {
            "// AWAITING INPUT",
            $"Walk up to the container and press [{Key(OptionsManager.Interact)}]."
        });

        // INVENTORY OPEN
        yield return TypeLines(new string[] {
            "// INVENTORY SYSTEM ONLINE",
            $"Press [{Key(OptionsManager.Inventory)}] to access your inventory.",
            "Items can be managed from this interface."
        });
        yield return WaitForCondition(PlayerPressedInventory, new string[] {
            "// AWAITING INPUT",
            $"Press [{Key(OptionsManager.Inventory)}] to open your inventory."
        });

        // INVENTORY CLOSE
        yield return TypeLines(new string[] {
            "// INVENTORY OPEN",
            $"Press [{Key(OptionsManager.Inventory)}] again to close it when ready."
        });
        yield return WaitForCondition(PlayerPressedInventory, new string[] {
            "// AWAITING INPUT",
            $"Press [{Key(OptionsManager.Inventory)}] to close the inventory."
        });

        // PAUSE OPEN
        yield return TypeLines(new string[] {
            "// PAUSE PROTOCOL",
            $"Press [{Key(OptionsManager.Pause)}] to access the system menu.",
            "Options and key bindings are available here."
        });
        yield return WaitForCondition(PlayerOpenedPause, new string[] {
            "// AWAITING INPUT",
            $"Press [{Key(OptionsManager.Pause)}] to open the pause menu."
        });

        // PAUSE CLOSE
        yield return TypeLines(new string[] {
            "// SYSTEM MENU OPEN",
            $"Press [{Key(OptionsManager.Pause)}] again to resume.",
            $"[{Key(OptionsManager.Pause)}] can always be used to pause or resume."
        });
        yield return WaitForCondition(PlayerClosedPause, new string[] {
            "// AWAITING INPUT",
            $"Press [{Key(OptionsManager.Pause)}] to close the pause menu and resume."
        });

        // COMPLETE
        yield return TypeLines(new string[] {
            "// TRAINING SEQUENCE COMPLETE",
            "All systems nominal.",
            "Proceed to the exit to enter the Main Hall."
        });
        yield return new WaitForSeconds(4f);
        yield return FadeTo(0f);

        tutorialDone = true;
        if (GameProgress.Instance != null)
            GameProgress.Instance.SetTutorialComplete();
    }

    private bool PlayerHasMoved()
    {
        if (playerBody == null) return false;
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
        return Input.GetKey(OptionsManager.Sprint) &&
               (Input.GetKey(OptionsManager.MoveForward) || Input.GetKey(OptionsManager.MoveBack) ||
                Input.GetKey(OptionsManager.MoveLeft) || Input.GetKey(OptionsManager.MoveRight));
    }

    private bool PlayerJumped() => Input.GetKeyDown(OptionsManager.Jump);
    private bool PlayerPressedInteract() => Input.GetKeyDown(OptionsManager.Interact);
    private bool PlayerPressedInventory() => Input.GetKeyDown(OptionsManager.Inventory);

    private bool PlayerOpenedPause()
    {
        bool isPaused = Time.timeScale == 0f;
        bool justOpened = isPaused && !pauseWasOpen;
        pauseWasOpen = isPaused;
        return justOpened;
    }

    private bool PlayerClosedPause()
    {
        bool isPaused = Time.timeScale == 0f;
        bool justClosed = !isPaused && pauseWasOpen;
        pauseWasOpen = isPaused;
        return justClosed;
    }

    private IEnumerator WaitForCondition(System.Func<bool> condition, string[] reminder)
    {
        float idleTimer = 0f;
        while (!condition())
        {
            if (Time.timeScale > 0f) idleTimer += Time.deltaTime;
            if (idleTimer >= 10f)
            {
                idleTimer = 0f;
                if (displayCoroutine != null) StopCoroutine(displayCoroutine);
                displayCoroutine = StartCoroutine(TypeLines(reminder));
            }
            yield return null;
        }
    }

    private IEnumerator TypeLines(string[] lines)
    {
        if (tutorialText != null) tutorialText.text = "";
        if (tutorialCanvasGroup != null) tutorialCanvasGroup.alpha = 1f;
        if (audioSource != null && typingSound != null)
        {
            audioSource.clip = typingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        for (int i = 0; i < lines.Length; i++)
        {
            foreach (char c in lines[i])
            {
                tutorialText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
            if (i < lines.Length - 1)
            {
                yield return new WaitForSeconds(linePause);
                tutorialText.text += "\n";
            }
        }
        if (audioSource != null) audioSource.Stop();
    }

    private IEnumerator FadeTo(float target)
    {
        if (tutorialCanvasGroup == null) yield break;
        while (Mathf.Abs(tutorialCanvasGroup.alpha - target) > 0.01f)
        {
            tutorialCanvasGroup.alpha = Mathf.MoveTowards(tutorialCanvasGroup.alpha, target, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        tutorialCanvasGroup.alpha = target;
    }
}