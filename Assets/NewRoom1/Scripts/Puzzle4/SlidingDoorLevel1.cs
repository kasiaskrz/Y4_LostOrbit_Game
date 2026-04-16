using System.Collections.Generic;
using UnityEngine;

public class SlidingDoorLevel1 : MonoBehaviour, IInteractable
{
    [Header("Order puzzle")]
    public SimpleButton[] correctOrder;

    [Header("Door movement")]
    public Vector3 openOffset = new Vector3(0f, 4f, 0f);
    public float openSpeed = 2f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip openSound;

    private readonly List<SimpleButton> pressedSequence = new List<SimpleButton>();
    private bool opened = false;
    private bool soundPlayed = false;
    private bool isUnlocked = false;

    private Vector3 closedLocalPos;
    private Vector3 openLocalPos;

    public string PromptText => opened ? "" : isUnlocked ? "Open Door" : "Locked - Solve the puzzle first";

    void Start()
    {
        closedLocalPos = transform.localPosition;
        openLocalPos = closedLocalPos + openOffset;
    }

    // Called by FuseBoxInteract when wire puzzle is solved
    public void Unlock()
    {
        isUnlocked = true;
    }

    public void Interact()
    {
        if (!isUnlocked || opened) return;
        opened = true;

        // disable door collider when opened
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (!soundPlayed && audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound, 5f);
            soundPlayed = true;
        }
    }

    // Called by buttons when pressed (existing puzzle)
    public void ButtonPressed(SimpleButton button)
    {
        if (opened) return;
        if (pressedSequence.Contains(button)) return;

        pressedSequence.Add(button);

        if (pressedSequence.Count == correctOrder.Length)
        {
            if (IsCorrectSequence())
            {
                opened = true;

                if (!soundPlayed && audioSource && openSound)
                {
                    audioSource.PlayOneShot(openSound, 5f);
                    soundPlayed = true;
                }
            }
            else
            {
                ResetButtons();
            }
        }
    }

    void Update()
    {
        if (!opened) return;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            openLocalPos,
            Time.deltaTime * openSpeed
        );
    }

    bool IsCorrectSequence()
    {
        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (pressedSequence[i] != correctOrder[i])
                return false;
        }
        return true;
    }

    void ResetButtons()
    {
        foreach (var b in correctOrder)
        {
            if (b != null)
                b.ResetButton();
        }

        pressedSequence.Clear();
    }
}