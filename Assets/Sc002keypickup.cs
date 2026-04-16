using UnityEngine;

public class SC002KeyPickup : MonoBehaviour
{
    [Tooltip("Assign the RoomUIManager in SC002.")]
    public RoomUIManager roomUIManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("SC002: Key collected!");

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.keyCollected = true;
            GameProgress.Instance.sc002Complete = true;
        }

        if (roomUIManager != null)
            roomUIManager.RoomCompleted();

        // Unlock the exit door
        FinishTrigger finishTrigger = FindFirstObjectByType<FinishTrigger>();
        if (finishTrigger != null)
            finishTrigger.EnableFinishZone();
        else
            Debug.LogWarning("SC002KeyPickup: No FinishTrigger found in scene.");

        gameObject.SetActive(false);
    }
}