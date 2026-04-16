using UnityEngine;
using TMPro;

public class LevelRunReporter : MonoBehaviour
{
    public int levelNumber = 1;
    public TextMeshProUGUI timerText;

    public float elapsed;
    public bool running; 
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
        if (NotePickup.IsOpen) return; // truly pause while reading note

        elapsed += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelTimer()
    {
        elapsed = 0f;
        running = true;

        Debug.Log($"Level {levelNumber} timer started.");
    }

    public void StopTimer()
    {
        if (!running) return;
        running = false;

        int timeMs = Mathf.RoundToInt(elapsed * 1000f);

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