using System.Collections.Generic;
using UnityEngine;

public class DoorsUnlockedButtons : MonoBehaviour
{
    [Header("Order puzzle")]
    public SimpleButton[] correctOrder; // Set size to 5 in Inspector

    [Header("Door movement")]
    public Vector3 openOffset = new Vector3(0f, 4f, 0f); // Y = up
    public float openSpeed = 2f;

    private readonly List<SimpleButton> pressedSequence = new List<SimpleButton>();
    private bool opened = false;

    private Vector3 closedLocalPos;
    private Vector3 openLocalPos;

    void Start()
    {
        // Use LOCAL position (important if door has parent)
        closedLocalPos = transform.localPosition;
        openLocalPos = closedLocalPos + openOffset;
    }

    // Called by buttons when pressed
    public void ButtonPressed(SimpleButton button)
    {
        if (opened) return;

        // Ignore double pressing same button
        if (pressedSequence.Contains(button)) return;

        pressedSequence.Add(button);

        if (pressedSequence.Count == correctOrder.Length)
        {
            if (IsCorrectSequence())
            {
                opened = true;
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

        // Smooth slide up
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
