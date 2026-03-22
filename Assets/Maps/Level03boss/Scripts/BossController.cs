using UnityEngine;

public class BossController : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Shield")]
    public GameObject shieldSphere;
    private bool shieldActive = false;

    [Header("Phase Nodes")]
    public PowerNode[] phaseNodes;
    private int currentPhaseIndex = 0;

    [Header("Phase Thresholds")]
    public int[] phaseThresholds = new int[] { 75, 50, 25 };

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (shieldSphere != null)
            shieldSphere.SetActive(false);

        for (int i = 0; i < phaseNodes.Length; i++)
        {
            if (phaseNodes[i] != null)
            {
                phaseNodes[i].SetInactiveState();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        if (shieldActive) return;

        currentHealth -= Mathf.RoundToInt(amount);
        currentHealth = Mathf.Max(currentHealth, 0);

        CheckPhaseTrigger();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void CheckPhaseTrigger()
    {
        if (currentPhaseIndex >= phaseThresholds.Length)
            return;

        if (currentHealth <= phaseThresholds[currentPhaseIndex])
        {
            ActivateShieldPhase();
        }
    }

    void ActivateShieldPhase()
    {
        shieldActive = true;

        if (shieldSphere != null)
            shieldSphere.SetActive(true);

        if (currentPhaseIndex < phaseNodes.Length && phaseNodes[currentPhaseIndex] != null)
        {
            phaseNodes[currentPhaseIndex].ActivateNode(this);
        }

        currentPhaseIndex++;
    }

    public void DisableShield()
    {
        shieldActive = false;

        if (shieldSphere != null)
            shieldSphere.SetActive(false);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Boss Defeated");
    }
}