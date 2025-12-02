using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction")]
    public float interactDistance = 2f;          // how close you need to be
    public KeyCode interactKey = KeyCode.E;     // key to press

    [Header("UI")]
    public GameObject interactPrompt;           // the "Press E" text object

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

                // If in range and key pressed -> kill enemy
                if (Input.GetKeyDown(interactKey))
                {
                    closestEnemy.KillInstantly();
                }
            }
        }

        // Show / hide the prompt
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(canInteract);
        }
    }

    EnemyHealth FindClosestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();

        EnemyHealth closest = null;
        float minDist = Mathf.Infinity;

        foreach (EnemyHealth enemy in enemies)
        {
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
