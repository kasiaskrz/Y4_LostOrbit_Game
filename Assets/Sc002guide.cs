using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Place on an empty GameObject in SC002.
/// Guides the player through: find box → push to spot → collect key → exit.
/// </summary>
public class SC002Guide : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI hintText;
    public CanvasGroup hintCanvasGroup;

    [Header("References")]
    [Tooltip("The movable box in SC002.")]
    public MovableBox movableBox;

    [Header("Fade")]
    public float fadeSpeed = 3f;

    private Coroutine currentCoroutine;
    private Coroutine idleCoroutine;

    private void Start()
    {
        if (hintCanvasGroup != null)
            hintCanvasGroup.alpha = 0f;

        StartCoroutine(RunGuide());
    }

    private IEnumerator RunGuide()
    {
        yield return new WaitForSeconds(1f);

        // Step 1 — Find and push the box
        yield return ShowText("Welcome to the puzzle room.\nFind the box and walk up to it.\nPress [E] to push it to the marked spot.");
        yield return WaitForCondition(() => movableBox != null && movableBox.movementFinished,
            "Find the box and press [E] to push it\nto the black spot on the floor.");

        // Step 2 — Collect the key
        yield return CrossfadeTo("The key has appeared!\nGo and collect it.");
        yield return WaitForCondition(() => GameProgress.Instance != null && GameProgress.Instance.sc002Complete,
            "Find the key and walk over it\nto collect it.");

        // Step 3 — Exit
        yield return CrossfadeTo("Key collected!\nHead back to the Main Hall\nthrough the exit door.");
    }

    private IEnumerator WaitForCondition(System.Func<bool> condition, string reminder)
    {
        float idleTimer = 0f;

        while (!condition())
        {
            if (Time.timeScale > 0f)
                idleTimer += Time.deltaTime;

            if (idleTimer >= 10f)
            {
                idleTimer = 0f;
                if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(FlashReminder(reminder));
            }

            yield return null;
        }
    }

    private IEnumerator ShowText(string message)
    {
        if (hintText != null) hintText.text = message;
        yield return FadeTo(1f);
    }

    private IEnumerator CrossfadeTo(string message)
    {
        yield return FadeTo(0f);
        yield return new WaitForSeconds(0.2f);
        if (hintText != null) hintText.text = message;
        yield return FadeTo(1f);
    }

    private IEnumerator FlashReminder(string message)
    {
        yield return FadeTo(0f);
        if (hintText != null) hintText.text = message;
        yield return FadeTo(1f);
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