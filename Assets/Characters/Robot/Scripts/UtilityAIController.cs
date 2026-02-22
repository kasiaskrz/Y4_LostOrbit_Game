using System.Collections.Generic;
using UnityEngine;

public class UtilityAIController : MonoBehaviour
{
    public EnemyBlackboard bb;

    [Header("Actions (drag ScriptableObject assets here)")]
    public List<UtilityAction> actions = new List<UtilityAction>();

    [Header("Think")]
    public float thinkInterval = 0.2f;

    UtilityAction _current;
    float _nextThink;

    void Reset()
    {
        bb = GetComponent<EnemyBlackboard>();
    }

    void Update()
    {
        if (!bb || !bb.sensors || !bb.agent || !bb.animator) return;

        if (Time.time >= _nextThink)
        {
            _nextThink = Time.time + thinkInterval;
            ChooseAction();
        }

        if (_current != null)
        {
            bool done = _current.Tick(bb);
            if (done)
            {
                _current.OnExit(bb);
                _current = null;
            }
        }
    }

    void ChooseAction()
    {
        UtilityAction best = null;
        float bestScore = 0f;

        foreach (var a in actions)
        {
            if (!a) continue;
            if (!a.CanChoose()) continue;

            float s = a.Score(bb) * a.weight;
            if (s > bestScore)
            {
                bestScore = s;
                best = a;
            }
        }

        if (best == null) return;

        if (best != _current)
        {
            _current?.OnExit(bb);
            _current = best;
            _current.MarkChosen();
            _current.OnEnter(bb);
        }
    }
}
