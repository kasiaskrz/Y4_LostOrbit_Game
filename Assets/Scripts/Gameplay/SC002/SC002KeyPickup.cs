using UnityEngine;

public class SC002KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("SC002 KEY COLLECTED!");

        GameProgress.Instance.keyCollected = true;

        // Find the door in Main Hall
        DoorWithLock door = FindFirstObjectByType<DoorWithLock>();
        if (door != null)
            door.UnlockDoor();

        gameObject.SetActive(false);
    }
}
