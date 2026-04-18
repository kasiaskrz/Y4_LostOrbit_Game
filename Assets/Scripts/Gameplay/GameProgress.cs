using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance { get; private set; }

    public bool boxPuzzleSolved = false;
    public bool keyCollected = false;
    public bool sc002Complete = false;
    public bool sc003Complete = false;
    public bool tutorialComplete = false;

    // 0 = no keys, 1 = one key, 2 = both keys
    public int keysCollected = 0;

    public bool CanAccessSC005 => sc002Complete && sc003Complete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}