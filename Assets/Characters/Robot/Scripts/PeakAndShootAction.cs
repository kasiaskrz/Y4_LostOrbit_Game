using UnityEngine;

[CreateAssetMenu(menuName = "AI/Utility Action/Peek And Shoot")]
public class PeekAndShootAction : UtilityAction
{
    public string popoutStateName = "popout";
    public string coverIdleStateName = "cover";

    public float popoutDuration = 0.55f; // how long the popout anim takes
    public float shootMoment = 0.25f;    // when in the anim to fire

    float _t;
    bool _fired;

    public override float Score(EnemyBlackboard bb)
    {
        if (!bb.sensors || !bb.sensors.player) return 0f;

        // Only shoot if we have LOS and are in cover
        if (!bb.InCover) return 0f;
        if (!bb.sensors.HasLOS) return 0f;

        // Only shoot if in reasonable shotgun range
        float d = bb.sensors.DistanceToPlayer;
        if (d > bb.preferredRange * 1.6f) return 0.1f; // low desire if far
        if (d < 1.5f) return 0.2f; // too close, still might shoot

        // cooldown gate
        if (Time.time < bb.nextFireTime) return 0f;

        // higher score when closer to preferred range
        float rangeScore = 1f - Mathf.Abs(d - bb.preferredRange) / Mathf.Max(0.001f, bb.preferredRange);
        return Mathf.Clamp01(rangeScore);
    }

    public override void OnEnter(EnemyBlackboard bb)
    {
        _t = 0f;
        _fired = false;

        // pop out
        bb.animator.CrossFadeInFixedTime(popoutStateName, 0.06f);

        // face player quickly
        FacePlayer(bb, 0.35f);
    }

    public override bool Tick(EnemyBlackboard bb)
    {
        _t += Time.deltaTime;

        FacePlayer(bb, 0.2f);

        if (!_fired && _t >= shootMoment)
        {
            FireShotgun(bb);
            _fired = true;
            bb.nextFireTime = Time.time + bb.fireCooldown;
        }

        if (_t >= popoutDuration)
        {
            // return to cover idle
            bb.animator.CrossFadeInFixedTime(coverIdleStateName, 0.08f);
            return true;
        }

        return false;
    }

    static void FacePlayer(EnemyBlackboard bb, float turnSpeed)
    {
        if (!bb.sensors || !bb.sensors.player) return;

        Vector3 to = bb.sensors.player.position - bb.transform.position;
        to.y = 0f;
        if (to.sqrMagnitude < 0.0001f) return;

        Quaternion target = Quaternion.LookRotation(to.normalized, Vector3.up);
        bb.transform.rotation = Quaternion.Slerp(bb.transform.rotation, target, turnSpeed);
    }

    static void FireShotgun(EnemyBlackboard bb)
    {
        if (!bb.sensors || !bb.sensors.player) return;

        Vector3 origin = bb.muzzle ? bb.muzzle.position : (bb.transform.position + Vector3.up * 1.5f);
        Vector3 baseDir = (bb.sensors.player.position - origin).normalized;

        for (int i = 0; i < bb.pellets; i++)
        {
            Vector3 dir = ApplySpread(baseDir, bb.spreadDegrees);

            if (Physics.Raycast(origin, dir, out RaycastHit hit, bb.range, bb.hitMask, QueryTriggerInteraction.Ignore))
            {
                var dmg = hit.collider.GetComponentInParent<IDamageable>();
                if (dmg != null) dmg.TakeDamage(bb.damagePerPellet);
            }
        }
    }

    static Vector3 ApplySpread(Vector3 dir, float degrees)
    {
        float angle = degrees * Mathf.Deg2Rad;
        Vector2 r = Random.insideUnitCircle * Mathf.Tan(angle);
        // spread in local-ish space (approx)
        Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
        Vector3 up = Vector3.Cross(dir, right).normalized;
        return (dir + right * r.x + up * r.y).normalized;
    }

    public override void OnExit(EnemyBlackboard bb) { }
}
