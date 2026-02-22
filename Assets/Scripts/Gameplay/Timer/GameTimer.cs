using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    public float timeRemaining = 300f; 
    private bool isRunning = false;

    private float debugPrintCooldown = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            Debug.Log("TIMER ENDED");
        }

        debugPrintCooldown -= Time.deltaTime;
        if (debugPrintCooldown <= 0f)
        {
            Debug.Log("Time Left: " + Mathf.CeilToInt(timeRemaining));
            debugPrintCooldown = 1f;
        }
    }

    public void StartTimer()
    {
        isRunning = true;
        Debug.Log("TIMER STARTED");
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("TIMER STOPPED at " + Mathf.CeilToInt(timeRemaining));
    }
}
