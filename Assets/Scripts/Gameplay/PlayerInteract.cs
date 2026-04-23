using UnityEngine;
public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 2f;
    public GameObject interactPrompt;
    public string enemyTag = "Enemy";
    public LayerMask enemyMask = ~0;
    void Update()
    {
        EnemyHealth closest = FindClosestEnemy();
        bool canInteract = closest != null && Vector3.Distance(transform.position, closest.transform.position) <= interactDistance;
        if (canInteract && Input.GetKeyDown(OptionsManager.Interact))
        {
            Vector3 hitPoint = closest.transform.position + Vector3.up;
            Vector3 hitDir = (closest.transform.position - transform.position).normalized;
            closest.ApplyDamage(999999f, hitPoint, hitDir);
        }
        if (interactPrompt != null) interactPrompt.SetActive(canInteract);
    }
    EnemyHealth FindClosestEnemy()
    {
        EnemyHealth closest = null; float min = Mathf.Infinity;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(enemyTag))
        {
            var e = go.GetComponentInChildren<EnemyHealth>();
            if (!e) continue;
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < min) { min = d; closest = e; }
        }
        return closest;
    }
}