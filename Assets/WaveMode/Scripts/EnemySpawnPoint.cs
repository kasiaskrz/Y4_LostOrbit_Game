using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Tooltip("Optional weight. Higher = more likely to be chosen.")]
    public float weight = 1f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.6f);
    }
}
