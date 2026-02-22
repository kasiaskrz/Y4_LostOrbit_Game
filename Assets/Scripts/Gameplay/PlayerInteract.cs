using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction")]
    public float interactDistance = 2f;          // how close you need to be
    public KeyCode interactKey = KeyCode.E;      // key to press

    [Header("UI")]
    public GameObject interactPrompt;            // the "Press E" text object

    [Header("Targeting")]
    public string enemyTag = "Enemy";            // optional: tag enemies "Enemy" for performance
    public LayerMask enemyMask = ~0;             // optional: set to Enemy layer if you have one

    void Update()
    {
        EnemyHealth closestEnemy = FindClosestEnemy();
        bool canInteract = false;

        if (closestEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, closestEnemy.transform.position);

            if (distance <= interactDistance)
            {
                canInteract = true;

                if (Input.GetKeyDown(interactKey))
                {
                    // Instant kill -> triggers ragdoll + destroy timer
                    Vector3 hitPoint = closestEnemy.transform.position + Vector3.up * 1.0f;
                    Vector3 hitDir = (closestEnemy.transform.position - transform.position).normalized;
                    closestEnemy.ApplyDamage(999999f, hitPoint, hitDir);
                }
            }
        }

        if (interactPrompt != null)
            interactPrompt.SetActive(canInteract);
    }

    EnemyHealth FindClosestEnemy()
    {
        // If you tag enemies, this is much cheaper than FindObjectsOfType every frame.
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(enemyTag);

        EnemyHealth closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject go in enemyObjects)
        {
            if (!go) continue;

            var enemy = go.GetComponentInChildren<EnemyHealth>();
            if (!enemy) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }
}
