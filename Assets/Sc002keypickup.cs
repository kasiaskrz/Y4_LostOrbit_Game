using UnityEngine;

public class SC002KeyPickup : MonoBehaviour
{
    [Tooltip("Assign the SC002Guide in SC002.")]
    public SC002Guide roomGuide;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("SC002: Key collected!");

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.keyCollected = true;
            GameProgress.Instance.sc002Complete = true;
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