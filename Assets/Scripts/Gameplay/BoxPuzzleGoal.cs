using UnityEngine;
using UnityEngine.Events;

public class BoxPuzzleGoal : MonoBehaviour
{
    [Tooltip("Tag of the box that needs to be here")]
    public string requiredTag = "MovableBox";

    [Tooltip("Called once when the box first reaches this area")]
    public UnityEvent onPuzzleSolved;

    private bool solved = false;

    private void OnTriggerEnter(Collider other)
    {
        if (solved) return;

        if (other.CompareTag(requiredTag))
        {
            solved = true;
            Debug.Log("Box puzzle solved!");

            if (GameProgress.Instance != null)
                GameProgress.Instance.boxPuzzleSolved = true;

            onPuzzleSolved?.Invoke(); // optional: play sound, light, UI, etc.
        }
    }
}
