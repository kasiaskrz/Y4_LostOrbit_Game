using System.Collections;
using UnityEngine;
using TMPro;

public class MainHallGuide : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI hintText;
    public CanvasGroup hintCanvasGroup;

    [Header("Locked Door Popup")]
    public GameObject finalDoor;

    [Header("Fade")]
    public float fadeSpeed = 3f;

    [Header("Typewriter")]
    public float typeSpeed = 0.04f;
    public float linePause = 0.3f;
    public AudioClip typingSound;
    public AudioSource audioSource;

    private Coroutine currentCoroutine;
    private int lastKeyCount = -1;

    private void Start()
    {
        if (hintCanvasGroup != null) hintCanvasGroup.alpha = 0f;
        StartCoroutine(GuideLoop());
    }

    private IEnumerator GuideLoop()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            int keys = GameProgress.Instance != null ? GameProgress.Instance.keysCollected : 0;
            if (keys != lastKeyCount)
            {
                lastKeyCount = keys;
                string[] message;
                if (keys == 0)
                    message = new string[] { "// SYSTEM ONLINE", "Two sectors are accessible.", "Retrieve both access keys to unlock the final corridor." };
                else if (keys == 1)
                    message = new string[] { "// ACCESS KEY ACQUIRED [ 1 / 2 ]", "One key fragment recovered.", "Locate the second key to proceed." };
                else
                    message = new string[] { "// ALL KEYS ACQUIRED [ 2 / 2 ]", "Final corridor unlocked.", "Proceed through the black door." };
                if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(ShowThenFade(message, 1f));
            }
            yield return new WaitForSeconds(10f);
            int currentKeys = GameProgress.Instance != null ? GameProgress.Instance.keysCollected : 0;
            string[] reminder;
            if (currentKeys == 0)
                reminder = new string[] { "// MISSION OBJECTIVE", "Retrieve access keys from Sector SC002 and SC003." };
            else if (currentKeys == 1)
                reminder = new string[] { "// MISSION OBJECTIVE", "One key fragment remaining.", "Search the other sector." };
            else
                reminder = new string[] { "// OBJECTIVE COMPLETE", "Proceed through the final corridor." };
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(ShowThenFade(reminder, 1f));
        }
    }

    public void ShowLockedDoorMessage()
    {
        int keys = GameProgress.Instance != null ? GameProgress.Instance.keysCollected : 0;
        string[] msg = keys == 0
            ? new string[] { "// ACCESS DENIED", "Two key fragments required.", "Search Sector SC002 and SC003." }
            : new string[] { "// ACCESS DENIED", "One key fragment still missing.", "Complete your search." };
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ShowThenFade(msg, 1f));
    }

    private IEnumerator ShowThenFade(string[] lines, float duration)
    {
        if (hintText != null) hintText.text = "";
        if (hintCanvasGroup != null) hintCanvasGroup.alpha = 1f;
        yield return StartCoroutine(TypeWriterLines(lines));
        yield return new WaitForSeconds(duration);
        yield return FadeTo(0f);
    }

    private IEnumerator TypeWriterLines(string[] lines)
    {
        if (hintText == null) yield break;
        hintText.text = "";
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
                hintText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
            if (i < lines.Length - 1)
            {
                yield return new WaitForSeconds(linePause);
                hintText.text += "\n";
            }
        }
        if (audioSource != null) audioSource.Stop();
    }

    private IEnumerator FadeTo(float target)
    {
        if (hintCanvasGroup == null) yield break;
        while (Mathf.Abs(hintCanvasGroup.alpha - target) > 0.01f)
        {
            hintCanvasGroup.alpha = Mathf.MoveTowards(hintCanvasGroup.alpha, target, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        hintCanvasGroup.alpha = target;
    }
}