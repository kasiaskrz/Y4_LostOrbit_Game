using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float typingSpeed = 0.05f;
    public float startDelay = 0.5f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip typingLoopSound;
    [Range(0f, 1f)] public float volume = 0.5f;

    private string fullText;
    private bool isFinished = false;

    public bool IsFinishedTyping()
    {
        return isFinished;
    }

    void Start()
    {
        isFinished = false;
        fullText = textComponent.text;
        textComponent.text = "";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        yield return new WaitForSeconds(startDelay);

        // ▶️ START LOOP
        if (audioSource != null && typingLoopSound != null)
        {
            audioSource.clip = typingLoopSound;
            audioSource.volume = volume;
            audioSource.loop = true;
            audioSource.Play();
        }

        foreach (char letter in fullText.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // ⛔ STOP LOOP
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        isFinished = true;
    }
}