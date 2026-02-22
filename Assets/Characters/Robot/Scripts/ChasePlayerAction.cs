using UnityEngine;

[CreateAssetMenu(menuName = "AI/Utility Action/Chase Player")]
public class ChasePlayerAction : UtilityAction
{
    public string walkStateName = "loco_forward";
    public float stopDistance = 10f;

    public override float Score(EnemyBlackboard bb)
    {
        if (!bb.sensors || !bb.sensors.player) return 0f;

        float d = bb.sensors.DistanceToPlayer;

        // chase if too far OR we lost LOS but recently saw them
        bool tooFar = d > stopDistance;
        bool lostButRecent = !bb.sensors.HasLOS && bb.sensors.TimeSinceSeen < 2.5f;

        if (!tooFar && !lostButRecent) return 0f;

        // don’t chase if we’re already in cover and can shoot soon
        if (bb.InCover && bb.sensors.HasLOS) return 0.05f;

        float s = 0.7f;
        if (lostButRecent) s = 0.9f;
        if (tooFar) s = Mathf.Clamp01((d - stopDistance) / 10f) + 0.3f;

        return Mathf.Clamp01(s);
    }

    public override void OnEnter(EnemyBlackboard bb)
    {
        bb.agent.isStopped = false;
        bb.animator.CrossFadeInFixedTime(walkStateName, 0.1f);
    }

    public override bool Tick(EnemyBlackboard bb)
    {
        if (!bb.sensors || !bb.sensors.player) return true;

        Vector3 target = bb.sensors.HasLOS ? bb.sensors.player.position : bb.sensors.LastSeenPos;

        if (Time.time >= bb.nextRepathTime)
        {
            bb.agent.SetDestination(target);
            bb.nextRepathTime = Time.time + bb.coverRepathCooldown;
        }

        float d = Vector3.Distance(bb.transform.position, bb.sensors.player.position);
        if (d <= stopDistance)
            return true;

        return false;
    }

    public override void OnExit(EnemyBlackboard bb) { }
}
