using UnityEngine;

public class EnemyRunAway : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float detectionRange = 10f;

    void Update()
    {
        if (player == null) return;

        // Distance from enemy to player
        float dist = Vector3.Distance(transform.position, player.position);

        // If player is close, move away
        if (dist < detectionRange)
        {
            // Direction *away* from the player
            Vector3 dir = (transform.position - player.position).normalized;

            // Move away
            transform.position += dir * speed * Time.deltaTime;
        }
    }
}
