using UnityEngine;


/// Place on the black goal spot in SC002.
/// When the movable box reaches it, the key appears.

public class SC002Complete : MonoBehaviour
{
    [Tooltip("The key GameObject to activate when box reaches the spot.")]
    public GameObject keyObject;

    [Tooltip("Assign SC002Guide to show message when box arrives.")]
    public SC002Guide roomGuide;

    [Tooltip("Tag on the movable box.")]
    public string boxTag = "MovableBox";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(boxTag)) return;

        triggered = true;
        Debug.Log("SC002: Box reached the goal spot!");

        if (keyObject != null)
            keyObject.SetActive(true);

        if (roomGuide != null)
            roomGuide.OnKeyCollected();
    }
}