using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    [Tooltip("How far this point provides cover from bullets/LOS checks. Used as a heuristic.")]
    public float coverRadius = 0.6f;

    [Tooltip("Optional: how good this cover is (higher = preferred).")]
    public float quality = 1f;

    [HideInInspector] public bool reserved;

    void OnDrawGizmos()
    {
        Gizmos.color = reserved ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.15f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
    }
}
