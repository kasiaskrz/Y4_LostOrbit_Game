using UnityEngine;

[ExecuteAlways]
public class LaserBeam : MonoBehaviour
{
    public Transform beamOrigin;
    public float maxDistance = 50f;
    public LineRenderer line;

    [Header("Hit light (optional)")]
    public Light hitLight;
    public float hitLightIntensity = 3f;

    private BeamSensor lastSensorHit;
    private Collider[] selfColliders;

    void OnEnable()
    {
        Init();
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (line == null) line = GetComponent<LineRenderer>();
        if (line != null) line.positionCount = 2;

        // IMPORTANT: get colliders on THIS laser object and its children
        // (so the ray ignores itself)
        selfColliders = GetComponentsInChildren<Collider>(true);

        if (hitLight != null)
        {
            hitLight.enabled = false;
            hitLight.intensity = hitLightIntensity;
        }
    }

    void Update()
    {
        if (beamOrigin == null) return;

        if (line == null || selfColliders == null)
            Init();

        if (line == null) return;

        Vector3 start = beamOrigin.position;
        Vector3 dir = beamOrigin.forward;

        line.SetPosition(0, start);

        RaycastHit[] hits = Physics.RaycastAll(start, dir, maxDistance, ~0, QueryTriggerInteraction.Ignore);

        bool found = false;
        RaycastHit bestHit = default;
        float bestDist = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (h.collider == null) continue;

            // ignore our own colliders
            bool isSelf = false;
            for (int c = 0; c < selfColliders.Length; c++)
            {
                var col = selfColliders[c];
                if (col != null && h.collider == col) { isSelf = true; break; }
            }
            if (isSelf) continue;

            if (h.distance < bestDist)
            {
                bestDist = h.distance;
                bestHit = h;
                found = true;
            }
        }

        BeamSensor sensorHitThisFrame = null;

        if (found)
        {
            line.SetPosition(1, bestHit.point);

            // sensor can be on parent even if collider is on child
            sensorHitThisFrame = bestHit.collider.GetComponentInParent<BeamSensor>();

            if (hitLight != null)
            {
                hitLight.enabled = true;
                hitLight.transform.position = bestHit.point;
                hitLight.color = (sensorHitThisFrame != null) ? Color.green : Color.yellow;
            }
        }
        else
        {
            line.SetPosition(1, start + dir * maxDistance);
            if (hitLight != null) hitLight.enabled = false;
        }

        if (lastSensorHit != null && lastSensorHit != sensorHitThisFrame)
            lastSensorHit.Deactivate();

        if (sensorHitThisFrame != null)
            sensorHitThisFrame.Activate();

        lastSensorHit = sensorHitThisFrame;
    }
}
