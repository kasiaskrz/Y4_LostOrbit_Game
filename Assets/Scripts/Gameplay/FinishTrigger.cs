using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private BoxCollider col;

    void Start()
    {
        col = GetComponent<BoxCollider>();

        // Before the key is collected, this should be a solid wall
        col.isTrigger = false;
    }

    public void EnableFinishZone()
    {
        // After key: now it becomes a trigger
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.instance.FinishLevel();
        }
    }
}
