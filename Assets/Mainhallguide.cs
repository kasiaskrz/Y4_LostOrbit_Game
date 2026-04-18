using System.Collections;
using UnityEngine;
using TMPro;


/// MainHall guide - updates messages based on keys collected.
/// Also handles locked door popup when player tries to enter SC005 without both keys.
public class MainHallGuide : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI hintText;
    public CanvasGroup hintCanvasGroup;

    [Header("Locked Door Popup")]
    [Tooltip("Assign the SC005 door trigger collider object.")]
    public GameObject finalDoor;

    [Header("Fade")]
    public float fadeSpeed = 3f;

    private Coroutine currentCoroutine;
    private int lastKeyCount = -1;

    private void Start()
    {
        if (hintCanvasGroup != null)
            hintCanvasGroup.alpha = 0f;

        StartCoroutine(GuideLoop());
    }

    private IEnumerator GuideLoop()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            int keys = GameProgress.Instance != null ? GameProgress.Instance.keysCollected : 0;

            // Only update if key count changed
            if (keys != lastKeyCount)
            {
                lastKeyCount = keys;

                string message;
                if (keys == 0)
                    message = "Welcome to the Main Hall.\nTwo rooms are available.\nFind the 2 keys to unlock the final door.";
                else if (keys == 1)
                    message = "Good work! You found 1 key.\nFind the last key in the other room\nto unlock the final door.";
                else
                    message = "Both keys collected!\nThe final door is now unlocked.\nHead through the black door.";

                if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(ShowThenFade(message, 6f));
            }

            yield return new WaitForSeconds(10f);

            // Repeat current relevant message every 10 seconds
            int currentKeys = GameProgress.Instance != null ? GameProgress.Instance.keysCollected : 0;
            string reminder;
            if (currentKeys == 0)
                reminder = "Find the 2 keys in SC002 and SC003\nto unlock the final door.";
            else if (currentKeys == 1)
                reminder = "Find the last key in the other room\nto unlock the final door.";
            else
                reminder = "Both keys collected!\nHead through the final door.";

            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(ShowThenFade(reminder, 5f));
        }
    }

    /// Call this from the final door trigger when player tries to enter without both keys
    public void ShowLockedDoorMessage()
    {
        int keys = GameProgress.Instance != null ? GameProgress.Instance.keysCollected : 0;
        string msg = keys == 0
            ? "Door locked!\nFind both keys in SC002 and SC003 first."
            : "Door locked!\nFind the last key to unlock this door.";

        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ShowThenFade(msg, 4f));
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