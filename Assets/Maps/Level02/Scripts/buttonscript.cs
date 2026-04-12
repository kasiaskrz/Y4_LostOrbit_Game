using UnityEngine;
using System.Collections;

public class LaserButton : MonoBehaviour
{
    public LaserPuzzleManager puzzleManager;
    public KeyCode interactKey = KeyCode.E;

    [Header("Visuals")]
    public Renderer buttonRenderer;
    public int materialIndex = 1;

    public Color defaultColor = Color.red;
    public Color correctColor = Color.green;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    private bool playerInRange = false;
    private bool hasBeenPressed = false;

    void Update()
    {
        if (playerInRange && !hasBeenPressed && Input.GetKeyDown(interactKey))
        {
            if (puzzleManager != null)
            {
                puzzleManager.PressButton(this);
            }
        }
    }

    public void SetCorrect()
    {
        hasBeenPressed = true;

        if (buttonRenderer != null)
        {
            Material[] mats = buttonRenderer.materials;
            mats[materialIndex].color = correctColor;
            buttonRenderer.materials = mats;
        }

        // 🟢 Play correct sound
        if (audioSource != null && correctSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(correctSound);
        }
    }

    public void ResetButton()
    {
        hasBeenPressed = false;

        if (buttonRenderer != null)
        {
            Material[] mats = buttonRenderer.materials;
            mats[materialIndex].color = defaultColor;
            buttonRenderer.materials = mats;
        }
    }

    public void PlayWrongFeedback()
    {
        // 🔴 Play wrong sound
        if (audioSource != null && wrongSound != null)
        {
            audioSource.pitch = 1f; // keep error consistent
            audioSource.PlayOneShot(wrongSound);
        }

        StartCoroutine(FlashWrong());
    }

    IEnumerator FlashWrong()
    {
        if (buttonRenderer == null) yield break;

        Material[] mats = buttonRenderer.materials;
        Color white = Color.white;

        for (int i = 0; i < 2; i++)
        {
            mats[materialIndex].color = white;
            buttonRenderer.materials = mats;
            yield return new WaitForSeconds(0.05f);

            mats[materialIndex].color = defaultColor;
            buttonRenderer.materials = mats;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}