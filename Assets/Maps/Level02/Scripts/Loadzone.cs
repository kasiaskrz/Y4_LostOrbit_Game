using UnityEngine;

public class LoadZone : MonoBehaviour
{
    [Header("Setup")]
    public string playerTag = "Player";

    [Header("Spawning")]
    public GameObject[] enemiesToSpawn;
    public Transform[] spawnPoints;

    [Header("Settings")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (triggerOnce && hasTriggered) return;

        hasTriggered = true;

        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (i < enemiesToSpawn.Length && enemiesToSpawn[i] != null)
            {
                Instantiate(enemiesToSpawn[i], spawnPoints[i].position, spawnPoints[i].rotation);
            }
        }
    }
}