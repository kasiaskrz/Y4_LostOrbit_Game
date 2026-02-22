using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;

    [Header("Spawn Points")]
    public List<EnemySpawnPoint> spawnPoints = new List<EnemySpawnPoint>();
    public bool autoFindSpawnPoints = true;

    [Header("Wave Settings")]
    public int startingEnemies = 3;
    public int enemiesAddedPerWave = 2;
    public float timeBetweenSpawns = 0.6f;
    public float timeBetweenWaves = 3f;

    [Header("Limits")]
    public int maxAliveEnemies = 10;

    [Header("UI (optional)")]
    public TMP_Text waveText;
    public TMP_Text aliveText;

    int waveNumber = 0;
    int aliveEnemies = 0;
    bool spawning;

    void OnEnable()
    {
        EnemyTracker.OnEnemySpawned += HandleEnemySpawned;
        EnemyTracker.OnEnemyDied += HandleEnemyDied;
    }

    void OnDisable()
    {
        EnemyTracker.OnEnemySpawned -= HandleEnemySpawned;
        EnemyTracker.OnEnemyDied -= HandleEnemyDied;
    }

    void Start()
    {
        if (autoFindSpawnPoints)
        {
            spawnPoints.Clear();
            spawnPoints.AddRange(FindObjectsOfType<EnemySpawnPoint>());
        }

        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            waveNumber++;
            int toSpawnThisWave = startingEnemies + (waveNumber - 1) * enemiesAddedPerWave;

            UpdateUI();

            // spawn enemies for this wave
            spawning = true;
            int spawned = 0;

            while (spawned < toSpawnThisWave)
            {
                // respect alive limit
                if (aliveEnemies < maxAliveEnemies)
                {
                    SpawnOne();
                    spawned++;
                }

                UpdateUI();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            spawning = false;

            // wait until all enemies are dead
            while (aliveEnemies > 0)
            {
                UpdateUI();
                yield return null;
            }

            // delay before next wave
            UpdateUI();
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnOne()
    {
        if (!enemyPrefab) return;
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("WaveSpawner: No spawn points set.");
            return;
        }

        EnemySpawnPoint sp = ChooseSpawnPointWeighted();
        if (!sp) sp = spawnPoints[Random.Range(0, spawnPoints.Count)];

        Instantiate(enemyPrefab, sp.transform.position, sp.transform.rotation);
    }

    EnemySpawnPoint ChooseSpawnPointWeighted()
    {
        float total = 0f;
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (spawnPoints[i]) total += Mathf.Max(0f, spawnPoints[i].weight);
        }

        if (total <= 0f) return null;

        float r = Random.value * total;
        float acc = 0f;

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            var sp = spawnPoints[i];
            if (!sp) continue;

            acc += Mathf.Max(0f, sp.weight);
            if (r <= acc) return sp;
        }

        return null;
    }

    void HandleEnemySpawned(EnemyTracker e)
    {
        aliveEnemies++;
        UpdateUI();
    }

    void HandleEnemyDied(EnemyTracker e)
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        UpdateUI();
    }

void UpdateUI()
{
    if (waveText) waveText.text = $"Wave: {waveNumber}";
    if (aliveText) aliveText.text = $"Alive: {aliveEnemies}";
}

}
