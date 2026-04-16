using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance { get; private set; }

    public bool boxPuzzleSolved = false;
    public bool keyCollected = false;
    public bool sc002Complete = false;
    public bool sc003Complete = false;

    // SC005 only unlocks when both rooms are done
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