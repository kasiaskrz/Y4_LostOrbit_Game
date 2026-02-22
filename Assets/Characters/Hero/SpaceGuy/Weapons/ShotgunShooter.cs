using UnityEngine;

public class ShotgunShooter : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;              // FPS camera
    public Transform muzzle;        // empty object at barrel
    public ShotTracer tracerPrefab; // prefab

    [Header("Shotgun")]
    public int pellets = 10;
    public float range = 60f;
    public float spreadDegrees = 4.5f;

    [Header("Damage (optional)")]
    public float damagePerPellet = 8f;
    public LayerMask hitMask = ~0;

    [Header("Stylised lines")]
    [Range(0f, 1f)] public float tracerChance = 0.8f; // 0.6-1 feels good
    public float tracerMuzzleForwardOffset = 0.05f;

    public void FireOnce()
    {
        if (!cam || !muzzle) return;

        Vector3 tracerStart = muzzle.position + muzzle.forward * tracerMuzzleForwardOffset;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 dir = GetSpreadDirection(cam.transform.forward);

            // Default end point
            Vector3 end = tracerStart + dir * range;

            // Raycast from camera so the shot goes where you aim
            if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
            {
                end = hit.point;

                // Optional damage
                var dmg = hit.collider.GetComponentInParent<IDamageable>();
                if (dmg != null) dmg.TakeDamage(damagePerPellet);
            }

            if (tracerPrefab && Random.value <= tracerChance)
            {
                ShotTracer tr = Instantiate(tracerPrefab);
                tr.Init(tracerStart, end);
            }
        }
    }

    Vector3 GetSpreadDirection(Vector3 forward)
    {
        float angle = spreadDegrees * Mathf.Deg2Rad;
        Vector2 r = Random.insideUnitCircle * Mathf.Tan(angle);
        return (forward + cam.transform.right * r.x + cam.transform.up * r.y).normalized;
    }
}

public interface IDamageable
{
    void TakeDamage(float amount);
}
