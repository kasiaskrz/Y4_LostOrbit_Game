using UnityEngine;

public class SC002KeyPickup : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            // Hide the key
            gameObject.SetActive(false);

            // Save that we got the key
            GameProgress.Instance.keyCollected = true;

            Debug.Log("SC002 KEY COLLECTED!");
        }
    }
}
