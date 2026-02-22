using UnityEngine;

public abstract class UtilityAction : ScriptableObject
{
    [Header("Tuning")]
    [Range(0f, 10f)] public float weight = 1f;
    [Tooltip("How often this action can be re-chosen (prevents spam).")]
    public float minChooseInterval = 0.15f;

    float _nextChooseTime;

    public bool CanChoose()
    {
        return Time.time >= _nextChooseTime;
    }

    public void MarkChosen()
    {
        _nextChooseTime = Time.time + minChooseInterval;
    }

    // Return a score (higher = more likely). 0 means "don't do me".
    public abstract float Score(EnemyBlackboard bb);

    // Called when this action becomes active
    public virtual void OnEnter(EnemyBlackboard bb) { }

    // Called every frame while active. Return true when finished.
    public abstract bool Tick(EnemyBlackboard bb);

    // Called when another action replaces this
    public virtual void OnExit(EnemyBlackboard bb) { }
}
