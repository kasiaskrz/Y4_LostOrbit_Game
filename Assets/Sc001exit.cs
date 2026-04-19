using System.Collections;
using UnityEngine;
using TMPro;

public class SC001Exit : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI hintText;
    public CanvasGroup hintCanvasGroup;

    [Header("Settings")]
    public float fadeSpeed = 3f;
    public float typeSpeed = 0.04f;
    public float linePause = 0.3f;
    public AudioClip typingSound;
    public AudioSource audioSource;

    private TeleportOnTrigger teleport;
    private Coroutine currentCoroutine;

    private void Start()
    {
        teleport = GetComponent<TeleportOnTrigger>();
        if (teleport != null) teleport.enabled = false;
        if (hintCanvasGroup != null) hintCanvasGroup.alpha = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        bool tutDone = GameProgress.Instance != null && GameProgress.Instance.tutorialComplete;

        if (!tutDone)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(ShowThenFade(new string[] {
                "// EXIT LOCKED",
                "Complete the training sequence before proceeding.",
                "Follow the on-screen instructions."
            }, 3f));
            return;
        }

        if (GameProgress.Instance != null)
            GameProgress.Instance.SetTutorialComplete();

        if (teleport != null)
        {
            teleport.enabled = true;
            teleport.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
        }
    }

    private IEnumerator ShowThenFade(string[] lines, float duration)
    {
        if (hintText != null) hintText.text = "";
        if (hintCanvasGroup != null) hintCanvasGroup.alpha = 1f;

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
                if (hintText != null) hintText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
            if (i < lines.Length - 1)
            {
                yield return new WaitForSeconds(linePause);
                if (hintText != null) hintText.text += "\n";
            }
        }

        if (audioSource != null) audioSource.Stop();
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