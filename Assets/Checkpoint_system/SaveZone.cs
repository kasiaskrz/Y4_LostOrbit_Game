using UnityEngine;

public class SaveZone : MonoBehaviour
{
    public bool saveOnlyOnce = true;
    private bool hasSaved = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (saveOnlyOnce && hasSaved) return;
        if (GameSaveManager.Instance == null) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        PlayerAmmo playerAmmo = other.GetComponent<PlayerAmmo>();
        Inventory inventory = other.GetComponent<Inventory>();

        if (playerHealth == null || playerAmmo == null || inventory == null)
        {
            Debug.LogWarning("Missing PlayerHealth, PlayerAmmo, or Inventory on player.");
            return;
        }

        GameSaveManager.Instance.SaveCheckpoint(
            other.transform,
            playerHealth,
            playerAmmo,
            inventory
        );

        hasSaved = true;
        Debug.Log("Save zone triggered.");
    }
}