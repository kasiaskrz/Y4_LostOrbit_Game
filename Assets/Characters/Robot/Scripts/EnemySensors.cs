using UnityEngine;

public class EnemySensors : MonoBehaviour
{
    public Transform player;
    public Transform eyes; // if null, uses transform
    public float sightRange = 25f;
    public float loseSightGrace = 1.0f;
    public LayerMask losMask = ~0;

    public bool HasLOS { get; private set; }
    public float DistanceToPlayer { get; private set; }
    public Vector3 LastSeenPos { get; private set; }
    public float TimeSinceSeen { get; private set; }

    void Update()
    {
        if (!player)
        {
            HasLOS = false;
            DistanceToPlayer = Mathf.Infinity;
            TimeSinceSeen += Time.deltaTime;
            return;
        }

        Vector3 eyePos = eyes ? eyes.position : transform.position + Vector3.up * 1.5f;
        Vector3 toPlayer = player.position - eyePos;
        DistanceToPlayer = toPlayer.magnitude;

        bool inRange = DistanceToPlayer <= sightRange;
        bool losNow = false;

        if (inRange)
        {
            Vector3 dir = toPlayer / Mathf.Max(0.0001f, DistanceToPlayer);
            if (Physics.Raycast(eyePos, dir, out RaycastHit hit, sightRange, losMask, QueryTriggerInteraction.Ignore))
            {
                losNow = hit.transform == player || hit.transform.IsChildOf(player);
            }
        }

        if (losNow)
        {
            HasLOS = true;
            LastSeenPos = player.position;
            TimeSinceSeen = 0f;
        }
        else
        {
            TimeSinceSeen += Time.deltaTime;
            if (TimeSinceSeen > loseSightGrace)
                HasLOS = false;
        }
    }
}
