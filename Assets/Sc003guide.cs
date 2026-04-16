using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Place on an empty GameObject in SC003.
/// Guides the player through: push box → collect key → exit unlocks.
/// </summary>
public class SC003Guide : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI hintText;
    public CanvasGroup hintCanvasGroup;

    [Header("References")]
    [Tooltip("The movable box in SC003.")]
    public MovableBox movableBox;

    [Header("Fade")]
    public float fadeSpeed = 3f;

    private Coroutine currentCoroutine;

    private void Start()
    {
        if (hintCanvasGroup != null)
            hintCanvasGroup.alpha = 0f;

        StartCoroutine(RunGuide());
    }

    private IEnumerator RunGuide()
    {
        yield return new WaitForSeconds(1f);

        // Step 1 — Push the box
        yield return ShowText("Find the box in this room.\nWalk up to it and press [E] to push it.\nThis will reveal the key.");
        yield return WaitForCondition(() => movableBox != null && movableBox.movementFinished,
            "Find the box and press [E] to push it.\nThe key is hidden nearby.");

        // Step 2 — Collect key
        yield return CrossfadeTo("The key has appeared!\nFind it and walk over it to collect it.\nThe exit will unlock once you have it.");
        yield return WaitForCondition(() => GameProgress.Instance != null && GameProgress.Instance.sc003Complete,
            "Find the key and walk over it\nto collect it and unlock the exit.");

        // Step 3 — Exit
        yield return CrossfadeTo("Key collected! The exit is now unlocked.\nHead back to the Main Hall.");
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