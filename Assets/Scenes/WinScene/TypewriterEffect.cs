using System.Collections;
using System.Text;
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
    private bool isStarted = false;  

    public bool IsFinishedTyping() => isFinished;

    void Start()
    {
        isFinished = false;
        isStarted = true;
        fullText = textComponent.text;
        textComponent.text = "";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        yield return new WaitForSeconds(startDelay);

        if (audioSource != null && typingLoopSound != null)
        {
            audioSource.clip = typingLoopSound;
            audioSource.volume = volume;
            audioSource.loop = true;
            audioSource.Play();
        }

        // Use StringBuilder to avoid repeated TMP layout rebuilds
        StringBuilder sb = new StringBuilder();
        foreach (char letter in fullText)
        {
            sb.Append(letter);
            textComponent.text = sb.ToString();  
            yield return new WaitForSeconds(typingSpeed);
        }

        if (audioSource != null)
            audioSource.Stop();

        isFinished = true;
    }
}