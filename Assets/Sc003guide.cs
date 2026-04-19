using System.Collections;
using UnityEngine;
using TMPro;

public class SC003Guide : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI hintText;
    public CanvasGroup hintCanvasGroup;

    [Header("References")]
    public MovableBox movableBox;
    public Camera playerCamera;
    public GameObject exitDoor;

    [Header("Settings")]
    public float welcomeDuration = 5f;
    public float raycastDistance = 3f;
    public float fadeSpeed = 3f;

    [Header("Typewriter")]
    public float typeSpeed = 0.04f;
    public float linePause = 0.3f;
    public AudioClip typingSound;
    public AudioSource audioSource;

    private bool welcomeShown = false;
    private bool boxPushed = false;
    private bool keyCollectedFlag = false;
    private Coroutine currentCoroutine;

    private void Start()
    {
        if (hintCanvasGroup != null) hintCanvasGroup.alpha = 0f;
        if (playerCamera == null) playerCamera = Camera.main;
        StartCoroutine(RunGuide());
    }

    private void Update()
    {
        if (keyCollectedFlag) return;
        if (welcomeShown && !boxPushed)
        {
            if (IsLookingAt(movableBox != null ? movableBox.gameObject : null))
            {
                if (hintText != null && hintText.text != "Press [E] to push the container.")
                {
                    if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                    hintText.text = "Press [E] to push the container.";
                }
                if (hintCanvasGroup != null && hintCanvasGroup.alpha < 1f)
                {
                    if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                    currentCoroutine = StartCoroutine(FadeTo(1f));
                }
                return;
            }
        }
        if (boxPushed && exitDoor != null)
        {
            if (IsLookingAt(exitDoor))
            {
                if (hintText != null && hintText.text != "// EXIT LOCKED\nKey fragment required to leave.")
                {
                    if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                    hintText.text = "// EXIT LOCKED\nKey fragment required to leave.";
                }
                if (hintCanvasGroup != null && hintCanvasGroup.alpha < 1f)
                {
                    if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                    currentCoroutine = StartCoroutine(FadeTo(1f));
                }
                return;
            }
        }
        if (welcomeShown && hintCanvasGroup != null && hintCanvasGroup.alpha > 0f)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(FadeTo(0f));
        }
    }

    private bool IsLookingAt(GameObject target)
    {
        if (playerCamera == null || target == null) return false;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            return hit.collider.gameObject == target || hit.collider.transform.IsChildOf(target.transform);
        return false;
    }

    private IEnumerator RunGuide()
    {
        yield return new WaitForSeconds(1f);
        if (GameProgress.Instance != null && GameProgress.Instance.sc003Complete)
        {
            int keys = GameProgress.Instance.keysCollected;
            string[] revisitMsg = keys >= 2
                ? new string[] { "// SECTOR SC003 — PREVIOUSLY CLEARED", "Both key fragments recovered.", "The final corridor awaits." }
                : new string[] { "// SECTOR SC003 — PREVIOUSLY CLEARED", "Key fragment already retrieved from this sector.", "Locate the remaining fragment in Sector SC002." };
            yield return StartCoroutine(ShowLines(revisitMsg, 4f));
            yield break;
        }
        yield return StartCoroutine(ShowLines(new string[] { "// SECTOR SC003 ONLINE", "Locate the container unit in this sector.", "Push it to reveal the key fragment." }, 0f));
        yield return new WaitForSeconds(welcomeDuration);
        yield return FadeTo(0f);
        welcomeShown = true;
        yield return new WaitUntil(() => movableBox == null || movableBox.movementFinished);
        boxPushed = true;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        yield return StartCoroutine(ShowLines(new string[] { "// ACCESS KEY DETECTED", "Key fragment revealed.", "Retrieve it to proceed." }, 1f));
        yield return FadeTo(0f);
    }

    public void ShowLockedExitMessage() { }

    public void OnKeyCollected()
    {
        keyCollectedFlag = true;
        StopAllCoroutines();
        StartCoroutine(ShowLines(new string[] { "// KEY FRAGMENT ACQUIRED", "Return to the Main Hall." }, 5f));
    }

    private IEnumerator ShowLines(string[] lines, float holdDuration)
    {
        if (hintText != null) hintText.text = "";
        if (hintCanvasGroup != null) hintCanvasGroup.alpha = 1f;
        yield return StartCoroutine(TypeWriterLines(lines));
        if (holdDuration > 0f) { yield return new WaitForSeconds(holdDuration); yield return FadeTo(0f); }
    }

    private IEnumerator TypeWriterLines(string[] lines)
    {
        if (hintText == null) yield break;
        hintText.text = "";
        if (audioSource != null && typingSound != null) { audioSource.clip = typingSound; audioSource.loop = true; audioSource.Play(); }
        for (int i = 0; i < lines.Length; i++)
        {
            foreach (char c in lines[i]) { hintText.text += c; yield return new WaitForSeconds(typeSpeed); }
            if (i < lines.Length - 1) { yield return new WaitForSeconds(linePause); hintText.text += "\n"; }
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