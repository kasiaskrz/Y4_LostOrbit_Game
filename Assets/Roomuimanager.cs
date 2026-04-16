using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Place in every scene on an empty GameObject.
/// Shows a sequence of entry messages one by one when the player enters,
/// then starts a repeating idle prompt every 10 seconds.
/// Call RoomCompleted() when the room objective is done.
/// </summary>
public class RoomUIManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("TMP text object for hints and prompts.")]
    public TextMeshProUGUI hintText;

    [Tooltip("CanvasGroup on the hint panel for fading.")]
    public CanvasGroup hintCanvasGroup;

    [Header("Entry Message Sequence")]
    [Tooltip("These messages play one after another when the player enters the room.")]
    public List<TimedMessage> entryMessages = new List<TimedMessage>();

    [Header("Idle Prompt")]
    [TextArea(2, 4)]
    [Tooltip("Shown every X seconds if the room is not complete.")]
    public string idlePrompt = "Hint: try pressing E near objects.";

    [Tooltip("How long the idle prompt stays visible.")]
    public float idlePromptDuration = 5f;

    [Tooltip("Seconds between idle prompts.")]
    public float idleInterval = 10f;

    [Header("Completion Message")]
    [TextArea(2, 4)]
    public string completionMessage = "Well done! Head to the exit.";

    [Tooltip("How long the completion message stays visible.")]
    public float completionMessageDuration = 5f;

    [Header("Fade")]
    public float fadeSpeed = 3f;

    private bool roomComplete = false;
    private Coroutine currentCoroutine;
    private Coroutine idleCoroutine;

    // ── Unity ────────────────────────────────────────────────────────────────

    private void Start()
    {
        if (hintCanvasGroup != null)
            hintCanvasGroup.alpha = 0f;

        if (entryMessages != null && entryMessages.Count > 0)
            currentCoroutine = StartCoroutine(PlayEntrySequence());
        else
            idleCoroutine = StartCoroutine(IdleLoop());
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>Call when room objective is complete.</summary>
    public void RoomCompleted()
    {
        if (roomComplete) return;
        roomComplete = true;

        if (idleCoroutine != null) StopCoroutine(idleCoroutine);
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowThenFade(completionMessage, completionMessageDuration, null));
    }

    /// <summary>Show a custom message immediately.</summary>
    public void ShowMessage(string message, float duration = 3f)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ShowThenFade(message, duration, null));
    }

    // ── Entry Sequence ───────────────────────────────────────────────────────

    private IEnumerator PlayEntrySequence()
    {
        foreach (TimedMessage msg in entryMessages)
        {
            yield return StartCoroutine(ShowThenFade(msg.message, msg.duration, null));
            yield return new WaitForSeconds(0.3f); // brief gap between messages
        }

        // Start idle loop after sequence finishes
        idleCoroutine = StartCoroutine(IdleLoop());
    }

    // ── Idle Loop ────────────────────────────────────────────────────────────

    private IEnumerator IdleLoop()
    {
        while (!roomComplete)
        {
            yield return new WaitForSeconds(idleInterval);
            if (roomComplete) yield break;

            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(ShowThenFade(idlePrompt, idlePromptDuration, null));
        }
    }

    // ── Display ──────────────────────────────────────────────────────────────

    private IEnumerator ShowThenFade(string message, float duration, System.Action onComplete)
    {
        if (hintText != null) hintText.text = message;

        yield return StartCoroutine(FadeTo(1f));
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(FadeTo(0f));

        onComplete?.Invoke();
    }

    private IEnumerator FadeTo(float target)
    {
        if (hintCanvasGroup == null) yield break;

        while (Mathf.Abs(hintCanvasGroup.alpha - target) > 0.01f)
        {
            hintCanvasGroup.alpha = Mathf.MoveTowards(
                hintCanvasGroup.alpha, target, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        hintCanvasGroup.alpha = target;
    }
}

/// <summary>A message with its own display duration.</summary>
[System.Serializable]
public class TimedMessage
{
    [TextArea(2, 4)]
    public string message;

    [Tooltip("How long this message stays visible in seconds.")]
    public float duration = 4f;
}