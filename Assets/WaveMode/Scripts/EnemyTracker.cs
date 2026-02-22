using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    public static System.Action<EnemyTracker> OnEnemySpawned;
    public static System.Action<EnemyTracker> OnEnemyDied;

    bool _reportedDeath;

    void OnEnable()
    {
        OnEnemySpawned?.Invoke(this);
    }

    // Call this when the enemy dies (we'll do it automatically below if you use EnemyHealthRagdoll)
    public void ReportDeath()
    {
        if (_reportedDeath) return;
        _reportedDeath = true;
        OnEnemyDied?.Invoke(this);
    }

    void OnDisable()
    {
        // If object gets disabled on death, still report once
        if (!_reportedDeath)
            ReportDeath();
    }
}
