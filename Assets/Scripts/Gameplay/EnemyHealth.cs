using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // We’ll call this directly when pressing E
    public void KillInstantly()
    {
        Die();
    }

    private void Die()
    {
        GameManager.Instance.WinGame();
        Destroy(gameObject);
    }
}
