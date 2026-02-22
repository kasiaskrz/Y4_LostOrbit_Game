using UnityEngine;

[CreateAssetMenu(menuName = "AI/Utility Action/Move To Cover")]
public class MoveToCoverAction : UtilityAction
{
    public string walkStateName = "loco_forward";
    public string enterCoverStateName = "loco_forward_to_cover";
    public string coverIdleStateName = "cover";

    public float arriveDistance = 0.6f;

    public override float Score(EnemyBlackboard bb)
    {
        if (!bb.currentCover) return 0f;

        // If already basically there, don't keep choosing.
        float dist = Vector3.Distance(bb.transform.position, bb.currentCover.transform.position);
        if (dist <= arriveDistance) return 0f;

        return 1f;
    }

    public override void OnEnter(EnemyBlackboard bb)
    {
        if (!bb.currentCover) return;

        if (Time.time >= bb.nextRepathTime)
        {
            bb.agent.isStopped = false;
            bb.agent.SetDestination(bb.currentCover.transform.position);
            bb.nextRepathTime = Time.time + bb.coverRepathCooldown;
        }

        // walking anim
        bb.animator.CrossFadeInFixedTime(walkStateName, 0.1f);
    }

    public override bool Tick(EnemyBlackboard bb)
    {
        if (!bb.currentCover) return true;

        float dist = Vector3.Distance(bb.transform.position, bb.currentCover.transform.position);
        if (dist > arriveDistance)
        {
            // keep moving
            if (!bb.agent.pathPending && bb.agent.remainingDistance > arriveDistance)
                return false;

            return false;
        }

        // arrived: play enter cover then settle into cover idle
        bb.agent.isStopped = true;

        bb.animator.CrossFadeInFixedTime(enterCoverStateName, 0.08f);
        bb.animator.CrossFadeInFixedTime(coverIdleStateName, 0.08f);

        // face the cover orientation
        bb.transform.rotation = Quaternion.Slerp(bb.transform.rotation, bb.currentCover.transform.rotation, 0.2f);

        return true;
    }

    public override void OnExit(EnemyBlackboard bb) { }
}
