using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// SC002 guide - welcome message fades after 60 seconds.
/// "Press E" hint only shows when player is looking at the movable box.
/// </summary>
public class SC002Guide : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI hintText;
    public CanvasGroup hintCanvasGroup;

    [Header("References")]
    public MovableBox movableBox;
    public Camera playerCamera;

    [Header("Settings")]
    public float welcomeDuration = 60f;
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
        if (!welcomeShown || boxPushed || keyCollectedFlag) return;

        // Check if player is looking at the movable box
        if (IsLookingAtBox())
        {
            if (hintCanvasGroup != null && hintCanvasGroup.alpha < 1f)
            {
                if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(FadeTo(1f));
                if (hintText != null) hintText.text = "Press [E] to push the box.";
            }
        }
        else
        {
            if (hintCanvasGroup != null && hintCanvasGroup.alpha > 0f)
            {
                if (currentCoroutine != null) StopCoroutine(currentCoroutine);
                currentCoroutine = StartCoroutine(FadeTo(0f));
            }
        }
    }

    private bool IsLookingAtBox()
    {
        if (playerCamera == null || movableBox == null) return false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            return hit.collider.gameObject == movableBox.gameObject ||
                   hit.collider.transform.IsChildOf(movableBox.transform);
        }
        return false;
    }

    private IEnumerator RunGuide()
    {
        yield return new WaitForSeconds(1f);

        // Step 1 — Entry message only, no instructions yet
        yield return ShowText("Welcome to the puzzle room.\nFind the box and walk up to it.");

        // Wait for welcome duration then fade out
        yield return new WaitForSeconds(welcomeDuration);
        yield return FadeTo(0f);

        welcomeShown = true;

        // Now Update() shows "Press E" when looking at box

        // Wait for box to be pushed
        yield return new WaitUntil(() => movableBox == null || movableBox.movementFinished);
        boxPushed = true;

        // Step 3 — Key appeared
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        yield return ShowText("The key has appeared!\nCollect the key to unlock the final room.");
        yield return new WaitForSeconds(5f);
        yield return FadeTo(0f);
    }

    // Called by FinishTrigger when player tries to leave without key
    public void ShowLockedExitMessage()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ShowThenFade("You need the key before you can leave!\nFind it and collect it first.", 4f));
    }

    // Called by SC002KeyPickup
    public void OnKeyCollected()
    {
        keyCollectedFlag = true;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
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