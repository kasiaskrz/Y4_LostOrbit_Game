using UnityEngine;
using System.Collections;

public class LaserPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Order")]
    public LaserButton[] correctOrder;

    [Header("Gate")]
    public LaserGate linkedGate;

    [Header("Fail Spawn")]
    public GameObject enemyPrefab;
    public Transform[] enemySpawnPoints;
    public Transform roomTarget;
    public int enemiesPerFail = 2;
    public bool spawnOnlyOnce = false;

    private int currentIndex = 0;
    private bool isResetting = false;
    private bool hasSpawnedOnce = false;

    public void PressButton(LaserButton button)
    {
        if (isResetting) return;

        if (correctOrder[currentIndex] == button)
        {
            // Correct button
            button.SetCorrect();

            currentIndex++;

            if (currentIndex >= correctOrder.Length)
            {
                // Puzzle complete
                if (linkedGate != null)
                {
                    linkedGate.DisableLaser();
                }
            }
        }
        else
        {
            // Wrong button
            button.PlayWrongFeedback();
            SpawnFailEnemies();
            StartCoroutine(ResetAfterFlash());
        }
    }

    private IEnumerator ResetAfterFlash()
    {
        isResetting = true;
        yield return new WaitForSeconds(0.2f);
        ResetPuzzle();
        isResetting = false;
    }

    void ResetPuzzle()
    {
        currentIndex = 0;

        foreach (var btn in correctOrder)
        {
            if (btn != null)
                btn.ResetButton();
        }
    }

    void SpawnFailEnemies()
    {
        if (spawnOnlyOnce && hasSpawnedOnce)
            return;

        if (enemyPrefab == null)
        {
            Debug.LogWarning("LaserPuzzleManager: No enemyPrefab assigned.");
            return;
        }

        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            Debug.LogWarning("LaserPuzzleManager: No enemySpawnPoints assigned.");
            return;
        }

        if (roomTarget == null)
        {
            Debug.LogWarning("LaserPuzzleManager: No roomTarget assigned.");
            return;
        }

        for (int i = 0; i < enemiesPerFail; i++)
        {
            Transform spawnPoint = enemySpawnPoints[i % enemySpawnPoints.Length];
            GameObject spawnedEnemy = Instantiate(
                enemyPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            EnemyRoomAttacker attacker = spawnedEnemy.GetComponent<EnemyRoomAttacker>();
            if (attacker != null)
            {
                attacker.SetTarget(roomTarget);
            }
        }

        hasSpawnedOnce = true;
    }
}