using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Mark key as collected
            GameProgress.Instance.keyCollected = true;

            // Unlock the door
            var door = Object.FindFirstObjectByType<FinishTrigger>();
            if (door != null)
                door.EnableFinishZone();

            // Hide the key after picking up
            gameObject.SetActive(false);

            Debug.Log("KEY COLLECTED!");
        }
    }
}
