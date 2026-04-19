using UnityEngine;

public class BossCore : MonoBehaviour, IDamageableBoss
{
    [Header("Health")]
    public int healthPerCycle = 70;
    public int finalHealth = 100;

    [Header("Damage Tuning")]
    [Tooltip("Multiplies incoming damage before it is applied.")]
    public float damageMultiplier = 1f;

    [Tooltip("Prevents one huge hit (like a full shotgun blast) from deleting a whole phase.")]
    public int maxDamagePerHit = 35;

    [Header("Shield")]
    public GameObject shieldVisual;

    [Header("Power Nodes Per Cycle")]
    public PowerNode[] cycle1Nodes;
    public PowerNode[] cycle2Nodes;
    public PowerNode[] cycle3Nodes;

    [Header("Optional - disable while shielded")]
    public MonoBehaviour[] disableWhileShielded;

    [Header("Optional - disable on death")]
    public MonoBehaviour[] disableOnDeath;

    [Header("Death")]
    public BossDeathBreakup deathBreakup;

    [Header("Debug")]
    public int currentCycle = 1;
    public bool shieldActive = false;
    public bool isDead = false;

    private int currentHealth;
    private int remainingNodesThisCycle = 0;

    private void Start()
    {
        currentHealth = healthPerCycle;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        SetNodesInactive(cycle1Nodes);
        SetNodesInactive(cycle2Nodes);
        SetNodesInactive(cycle3Nodes);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        if (shieldActive)
            return;

        int scaledDamage = Mathf.RoundToInt(damage * damageMultiplier);
        int finalDamage = Mathf.Clamp(scaledDamage, 0, maxDamagePerHit);

        currentHealth -= finalDamage;

        if (currentCycle <= 3)
        {
            if (currentHealth <= 0)
            {
                EnterShieldPhase();
            }
        }
        else
        {
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void EnterShieldPhase()
    {
        shieldActive = true;

        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        SetShieldedScripts(false);

        PowerNode[] activeCycleNodes = GetNodesForCurrentCycle();
        remainingNodesThisCycle = 0;

        if (activeCycleNodes == null || activeCycleNodes.Length == 0)
        {
            ExitShieldPhase();
            return;
        }

        for (int i = 0; i < activeCycleNodes.Length; i++)
        {
            if (activeCycleNodes[i] != null)
            {
                remainingNodesThisCycle++;
                activeCycleNodes[i].ActivateNode(this);
            }
        }

        if (remainingNodesThisCycle <= 0)
        {
            ExitShieldPhase();
        }
    }

    public void NotifyNodeDestroyed(PowerNode node)
    {
        if (isDead || !shieldActive)
            return;

        remainingNodesThisCycle--;

        if (remainingNodesThisCycle <= 0)
        {
            ExitShieldPhase();
        }
    }

    private void ExitShieldPhase()
    {
        shieldActive = false;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        SetShieldedScripts(true);

        currentCycle++;

        if (currentCycle <= 3)
        {
            currentHealth = healthPerCycle;
        }
        else
        {
            currentHealth = finalHealth;
        }
    }

    private PowerNode[] GetNodesForCurrentCycle()
    {
        switch (currentCycle)
        {
            case 1: return cycle1Nodes;
            case 2: return cycle2Nodes;
            case 3: return cycle3Nodes;
            default: return new PowerNode[0];
        }
    }

    private void SetNodesInactive(PowerNode[] nodes)
    {
        if (nodes == null)
            return;

        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] != null)
            {
                nodes[i].SetInactiveState();
            }
        }
    }

    private void SetShieldedScripts(bool enabledState)
    {
        if (disableWhileShielded == null)
            return;

        for (int i = 0; i < disableWhileShielded.Length; i++)
        {
            if (disableWhileShielded[i] != null)
                disableWhileShielded[i].enabled = enabledState;
        }
    }

    private void Die()
    {
        isDead = true;
        shieldActive = false;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        if (disableOnDeath != null)
        {
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                if (disableOnDeath[i] != null)
                    disableOnDeath[i].enabled = false;
            }
        }

        if (deathBreakup != null)
        {
            deathBreakup.PlayDeath();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}