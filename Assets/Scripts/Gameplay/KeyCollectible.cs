using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            // Hide the key (you can replace this with animation later)
            gameObject.SetActive(false);

            // Tell LevelManager that the key was collected
            LevelManager.instance.KeyCollected();
        }
    }
}
