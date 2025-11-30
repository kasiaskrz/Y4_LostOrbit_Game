using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    [Header("Door Parts")]
    public Animator doorAnimator;          // optional
    public Collider doorCollider;         // what blocks the player
    public TeleportOnTrigger teleporter;  // optional: teleport script on the doorway

    [Header("Settings")]
    public bool doorOpensWhenBoxPuzzleSolved = true;

    private void Start()
    {
        bool shouldBeOpen =
            GameProgress.Instance != null &&
            doorOpensWhenBoxPuzzleSolved &&
            GameProgress.Instance.boxPuzzleSolved;

        if (shouldBeOpen)
        {
            OpenDoorImmediate();
        }
        else
        {
            CloseDoor();
        }
    }

    public void OpenDoorImmediate()
    {
        if (doorAnimator != null)
        {
            // Use whatever animation setup you like:
            // e.g. doorAnimator.SetBool("Open", true);
            doorAnimator.Play("Open", 0, 1f); // jump to opened pose if you have an "Open" state
        }

        if (doorCollider != null)
            doorCollider.enabled = false;

        if (teleporter != null)
            teleporter.enabled = true;
    }

    public void CloseDoor()
    {
        if (doorAnimator != null)
        {
            // doorAnimator.SetBool("Open", false);
            doorAnimator.Play("Closed", 0, 1f); // or whatever your closed state is
        }

        if (doorCollider != null)
            doorCollider.enabled = true;

        if (teleporter != null)
            teleporter.enabled = false;
    }
}
