using UnityEngine;

public class SC003KeyPickup : MonoBehaviour
{
    [Tooltip("Assign the SC003Guide in SC003.")]
    public SC003Guide roomGuide;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("SC003: Key collected!");

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.keyCollected = true;
            GameProgress.Instance.sc003Complete = true;
            GameProgress.Instance.keysCollected++;
        }

        if (roomGuide != null)
            roomGuide.OnKeyCollected();

        // Unlock finish door in this scene
        FinishTrigger door = FindFirstObjectByType<FinishTrigger>();
        if (door != null) door.EnableFinishZone();

        gameObject.SetActive(false);
    }
}