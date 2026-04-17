using UnityEngine;

public class LevelStartLoader : MonoBehaviour
{
    public GameObject player;
    public Transform spawnPoint;

    private void Start()
    {
        if (player == null || spawnPoint == null) return;

        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;

        if (GameSaveManager.Instance != null && GameSaveManager.Instance.hasCheckpoint)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            PlayerAmmo playerAmmo = player.GetComponent<PlayerAmmo>();
            Inventory inventory = player.GetComponent<Inventory>();

            if (playerHealth != null)
            {
                playerHealth.currentHealth = GameSaveManager.Instance.currentSave.currentHealth;
                playerHealth.OnHealthChanged?.Invoke(playerHealth.currentHealth, playerHealth.maxHealth);
            }

            if (playerAmmo != null)
            {
                playerAmmo.shotgunAmmo = GameSaveManager.Instance.currentSave.shotgunAmmo;
                playerAmmo.rifleAmmo = GameSaveManager.Instance.currentSave.rifleAmmo;
            }

            GameSaveManager.Instance.LoadInventoryIntoExistingScript(inventory);
        }
    }
}