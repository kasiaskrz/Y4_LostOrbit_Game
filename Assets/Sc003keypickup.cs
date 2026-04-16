using UnityEngine;

/// <summary>
/// Place on the key collectible in SC003.
/// Collecting the key marks SC003 as complete and unlocks the exit.
/// </summary>
public class SC003KeyPickup : MonoBehaviour
{
    [Tooltip("Assign the RoomUIManager in SC003 to show completion message.")]
    public RoomUIManager roomUIManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("SC003: Key collected!");

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.keyCollected = true;
            GameProgress.Instance.sc003Complete = true;
        }

        if (roomUIManager != null)
            roomUIManager.RoomCompleted();

        // Unlock the exit door
        FinishTrigger finishTrigger = FindFirstObjectByType<FinishTrigger>();
        if (finishTrigger != null)
            finishTrigger.EnableFinishZone();
        else
            Debug.LogWarning("SC003KeyPickup: No FinishTrigger found in scene.");

        gameObject.SetActive(false);
    }
}