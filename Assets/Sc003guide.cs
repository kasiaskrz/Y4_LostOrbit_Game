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
    [Tooltip("Assign the exit door collider object.")]
    public GameObject exitDoor;

    [Header("Settings")]
    public float welcomeDuration = 5f;
    public float raycastDistance = 3f;
    public float fadeSpeed = 3f;

    private bool welcomeShown = false;
    private bool boxPushed = false;
    private bool keyCollectedFlag = false;
    private Coroutine currentCoroutine;

    private void Start()
    {
        if (hintCanvasGroup != null)
            hintCanvasGroup.alpha = 0f;

        if (playerCamera == null)
            playerCamera = Camera.main;

        StartCoroutine(RunGuide());
    }

    private void Update()
    {
        if (keyCollectedFlag) return;

        // Show press E when looking at box (before box is pushed)
        if (welcomeShown && !boxPushed)
        {
            if (IsLookingAt(movableBox != null ? movableBox.gameObject : null))
            {
                SetText("Press [E] to push the box.", true);
                return;
            }
        }

        // Show locked message when looking at exit door (before key collected)
        if (boxPushed && exitDoor != null)
        {
            if (IsLookingAt(exitDoor))
            {
                SetText("You need the key before you can leave!", true);
                return;
            }
        }

        // Nothing to show — fade out
        if (welcomeShown && hintCanvasGroup != null && hintCanvasGroup.alpha > 0f)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(FadeTo(0f));
        }
    }

    private void SetText(string message, bool show)
    {
        if (hintText != null && hintText.text != message)
        {
            hintText.text = message;
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        }
        if (show && hintCanvasGroup != null && hintCanvasGroup.alpha < 1f)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(FadeTo(1f));
        }
    }

    private bool IsLookingAt(GameObject target)
    {
        if (playerCamera == null || target == null) return false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            return hit.collider.gameObject == target ||
                   hit.collider.transform.IsChildOf(target.transform);
        }
        return false;
    }

    private IEnumerator RunGuide()
    {
        yield return new WaitForSeconds(1f);

        yield return ShowText("Find the box in this room.\nWalk up to it and push it to reveal the key.");
        yield return new WaitForSeconds(welcomeDuration);
        yield return FadeTo(0f);

        welcomeShown = true;

        yield return new WaitUntil(() => movableBox == null || movableBox.movementFinished);
        boxPushed = true;

        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        yield return ShowText("The key has appeared!\nCollect it.");
        yield return new WaitForSeconds(5f);
        yield return FadeTo(0f);
    }

    public void ShowLockedExitMessage() { } // Now handled by Update raycast

    public void OnKeyCollected()
    {
        keyCollectedFlag = true;
        StopAllCoroutines();
        StartCoroutine(ShowThenFade("Key collected!\nHead back to the Main Hall.", 5f));
    }

    private IEnumerator ShowText(string message)
    {
        if (hintText != null) hintText.text = message;
        yield return FadeTo(1f);
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