using UnityEngine;
using TMPro;

public class LevelRunReporter : MonoBehaviour
{
    public int levelNumber = 1;
    public TextMeshProUGUI timerText;

    float startTime;
    bool running;

    SupabaseAuth supa;

    void Awake()
    {
        supa = FindFirstObjectByType<SupabaseAuth>();
    }

    void Start()
    {
        StartLevelTimer();
    }

    void Update()
    {
        if (!running) return;

        float time = Time.time - startTime;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelTimer()
    {
        startTime = Time.time;
        running = true;

        Debug.Log($"Level {levelNumber} timer started.");
    }

    public void StopTimer()
    {
        if (!running) return;

        running = false;

        int timeMs = Mathf.RoundToInt((Time.time - startTime) * 1000f);

        Debug.Log($"Level {levelNumber} finished in {timeMs}ms");

        SubmitTime(timeMs);
    }

    void SubmitTime(int timeMs)
    {
        if (supa == null)
        {
            Debug.LogWarning("SupabaseAuth not found.");
            return;
        }

        if (string.IsNullOrEmpty(supa.accessToken) || string.IsNullOrEmpty(supa.userId))
        {
            Debug.Log("Guest mode: run not submitted.");
            return;
        }

        supa.SubmitRun(levelNumber, timeMs);
    }
}