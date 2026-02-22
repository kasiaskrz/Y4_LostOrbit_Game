using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Brief "item collected" notification that fades out. 
/// Place once in your Canvas and reference it as a singleton.
/// </summary>
public class PickupNotification : MonoBehaviour
{
    public static PickupNotification Instance { get; private set; }

    [Header("References")]
    public Image iconImage;
    public TextMeshProUGUI messageText;
    public CanvasGroup canvasGroup;

    [Header("Animation")]
    public float displayTime = 2f;
    public float fadeDuration = 0.4f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0;
    }

    public static void Show(Sprite icon, string itemName, int quantity)
    {
        if (Instance == null) return;
        Instance.ShowInternal(icon, itemName, quantity);
    }

    private void ShowInternal(Sprite icon, string itemName, int quantity)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        iconImage.sprite = icon;
        iconImage.enabled = icon != null;
        messageText.text = quantity > 1
            ? $"+ {quantity}x  {itemName}"
            : $"+  {itemName}";

        currentRoutine = StartCoroutine(AnimateNotification());
    }

    private IEnumerator AnimateNotification()
    {
        // Fade in
        float t = 0;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = t / fadeDuration;
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayTime);

        // Fade out
        t = 0;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = 1 - (t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}