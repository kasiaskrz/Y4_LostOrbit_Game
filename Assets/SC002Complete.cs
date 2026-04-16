using UnityEngine;

/// <summary>
/// Place on the black goal spot in SC002.
/// When the movable box reaches it, the key appears.
/// </summary>
public class SC002Complete : MonoBehaviour
{
    [Tooltip("The key GameObject to activate when box reaches the spot.")]
    public GameObject keyObject;

    [Tooltip("Assign the RoomUIManager in SC002.")]
    public RoomUIManager roomUIManager;

    [Tooltip("Tag on the movable box.")]
    public string boxTag = "MovableBox";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(boxTag)) return;

        triggered = true;
        Debug.Log("SC002: Box reached the goal spot!");

        // Show the key
        if (keyObject != null)
            keyObject.SetActive(true);

        // Show a hint to collect it
        if (roomUIManager != null)
            roomUIManager.ShowMessage("The key has appeared! Go collect it.", 4f);
    }
}