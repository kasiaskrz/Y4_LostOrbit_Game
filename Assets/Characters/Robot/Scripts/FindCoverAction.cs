using UnityEngine;

[CreateAssetMenu(menuName = "AI/Utility Action/Find Cover")]
public class FindCoverAction : UtilityAction
{
    public override float Score(EnemyBlackboard bb)
    {
        if (!bb.sensors || !bb.sensors.player) return 0f;

        // Want cover if we have LOS and we're not already in cover
        if (bb.InCover) return 0f;
        if (!bb.sensors.HasLOS) return 0f;

        // More urgent if player is within preferred shotgun range (dangerous)
        float d = bb.sensors.DistanceToPlayer;
        float urgency = Mathf.InverseLerp(bb.preferredRange * 1.5f, bb.tooCloseRange, d);
        return Mathf.Clamp01(urgency);
    }

    public override void OnEnter(EnemyBlackboard bb)
    {
        // find closest/best cover point within radius
        CoverPoint best = null;
        float bestScore = float.NegativeInfinity;

        var all = GameObject.FindObjectsOfType<CoverPoint>();
        foreach (var cp in all)
        {
            if (!cp || cp.reserved) continue;

            float dist = Vector3.Distance(bb.transform.position, cp.transform.position);
            if (dist > bb.coverSearchRadius) continue;

            // Prefer closer cover and higher quality
            float score = (cp.quality * 2f) - dist * 0.15f;

            // Prefer cover that breaks LOS (rough heuristic)
            if (bb.sensors.HasLOS)
            {
                Vector3 toPlayer = bb.sensors.player.position - cp.transform.position;
                // If the cover faces away from player, it might be a better "hide" spot
                float facing = Vector3.Dot(cp.transform.forward, toPlayer.normalized);
                score -= facing * 0.5f;
            }

            if (score > bestScore)
            {
                bestScore = score;
                best = cp;
            }
        }

        if (best)
        {
            best.reserved = true;
            bb.currentCover = best;
        }
    }

    public override bool Tick(EnemyBlackboard bb)
    {
        // This action just selects a cover point, then finishes.
        return true;
    }

    public override void OnExit(EnemyBlackboard bb) { }
}
