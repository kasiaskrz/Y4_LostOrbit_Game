using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Place on an empty GameObject in MainHall.
/// Shows guidance based on how many rooms the player has completed.
/// </summary>
public class MainHallGuide : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI hintText;
    public CanvasGroup hintCanvasGroup;

    [Header("Fade")]
    public float fadeSpeed = 3f;

    private Coroutine currentCoroutine;
    private int lastKnownProgress = -1;

    private void Start()
    {
        if (hintCanvasGroup != null)
            hintCanvasGroup.alpha = 0f;

        StartCoroutine(GuideLoop());
    }

    private IEnumerator GuideLoop()
    {
        // Small delay so scene finishes loading
        yield return new WaitForSeconds(1f);

        // Show entry message based on current progress
        yield return ShowCurrentMessage();

        // Keep checking progress every 10 seconds and update if changed
        while (true)
        {
            yield return new WaitForSeconds(10f);
            yield return ShowCurrentMessage();
        }
    }

    private IEnumerator ShowCurrentMessage()
    {
        if (GameProgress.Instance == null) yield break;

        bool sc002Done = GameProgress.Instance.sc002Complete;
        bool sc003Done = GameProgress.Instance.sc003Complete;

        string message;

        if (!sc002Done && !sc003Done)
        {
            message = "Welcome to the Main Hall.\nTwo rooms are available.\nComplete both to unlock the final door.";
        }
        else if (sc002Done && !sc003Done)
        {
            message = "Good work!\nNow find the key in SC003.\nCollect it to unlock the final door.";
        }
        else if (!sc002Done && sc003Done)
        {
            message = "Good work!\nNow complete the puzzle in SC002.\nCollect the key to unlock the final door.";
        }
        else
        {
            message = "Both rooms complete!\nThe final door is now unlocked.\nHead through the black door.";
        }

        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ShowThenFade(message, 5f));
        yield return currentCoroutine;
    }

    private IEnumerator ShowThenFade(string message, float duration)
    {
        if (hintText != null) hintText.text = message;
        yield return FadeTo(1f);
        yield return new WaitForSeconds(duration);
        yield return FadeTo(0f);
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