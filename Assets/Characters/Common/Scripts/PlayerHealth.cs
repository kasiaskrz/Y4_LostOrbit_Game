using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    public int currentHealth;
    bool isDead = false;

    public Action<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        int damage = Mathf.RoundToInt(amount);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died.");
        Debug.Log("hasCheckpoint = " + (GameSaveManager.Instance != null ? GameSaveManager.Instance.hasCheckpoint.ToString() : "no instance"));

        PlayerAmmo ammo = GetComponent<PlayerAmmo>();
        Inventory inventory = GetComponent<Inventory>();

        if (GameSaveManager.Instance != null && GameSaveManager.Instance.hasCheckpoint)
        {
            GameSaveManager.Instance.RespawnPlayer(
                gameObject,
                this,
                ammo,
                inventory
            );
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Going to LoseScene");
            PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();
            SceneManager.LoadScene("LoseScene");
        }
    }
}