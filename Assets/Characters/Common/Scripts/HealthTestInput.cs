using UnityEngine;

public class HealthTestInput : MonoBehaviour
{
    public PlayerHealth health;
    public int damageAmount = 10;
    public int healAmount = 10;

    void Awake()
    {
        if (!health) health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (!health) return;

        if (Input.GetKeyDown(KeyCode.H)) health.TakeDamage(damageAmount); // H = hurt
        if (Input.GetKeyDown(KeyCode.J)) health.Heal(healAmount);        // J = heal
        if (Input.GetKeyDown(KeyCode.K)) health.TakeDamage(9999);        // K = kill
    }
}
