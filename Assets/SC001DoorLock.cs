using System.Collections;
using UnityEngine;
using TMPro;

public class SC001DoorLock : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI hintText;
    public CanvasGroup hintCanvasGroup;

    [Header("Transition")]
    public string sceneToLoad = "SC001";
    public string targetSpawnID = "SC001Door";

    [Header("Settings")]
    public float fadeSpeed = 3f;

    private Coroutine currentCoroutine;
    private bool hasTriggered = false;

    private void Start()
    {
        if (hintCanvasGroup != null)
            hintCanvasGroup.alpha = 0f;

        TeleportOnTrigger t = GetComponent<TeleportOnTrigger>();
        if (t != null) t.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;

        if (GameProgress.Instance != null && GameProgress.Instance.tutorialComplete)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(ShowThenFade(
                "You just came from here!\nTry another room.", 4f));
            return;
        }

        hasTriggered = true;
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.TransitionToScene(sceneToLoad, targetSpawnID);
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