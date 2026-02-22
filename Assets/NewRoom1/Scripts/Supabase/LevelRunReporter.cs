using UnityEngine;

public class LevelRunReporter : MonoBehaviour
{
    public int levelNumber = 1;

    float startTime;
    bool running;

    SupabaseAuth supa;

    void Awake()
    {
        supa = FindFirstObjectByType<SupabaseAuth>();
    }

    public void StartLevelTimer()
    {
        startTime = Time.time;
        running = true;
        Debug.Log($"Level {levelNumber} timer started.");
    }

    public void FinishLevelAndSubmit()
    {
        if (!running)
        {
            Debug.LogWarning("FinishLevelAndSubmit called but timer wasn't running.");
            return;
        }

        running = false;

        int timeMs = Mathf.RoundToInt((Time.time - startTime) * 1000f);
        Debug.Log($"Level {levelNumber} finished in {timeMs}ms");

        if (supa == null)
        {
            Debug.LogWarning("SupabaseAuth not found. Not submitting.");
            return;
        }

        // Guest mode check: only submit if logged in
        if (string.IsNullOrEmpty(supa.accessToken) || string.IsNullOrEmpty(supa.userId))
        {
            Debug.Log("Guest mode: run not submitted.");
            return;
        }

        supa.SubmitRun(levelNumber, timeMs);
    }
}
