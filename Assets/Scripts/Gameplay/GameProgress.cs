using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance { get; private set; }

    public bool boxPuzzleSolved = false;
    public bool keyCollected = false;
    public bool sc002Complete = false;
    public bool sc003Complete = false;
    public bool tutorialComplete = false;
    public int keysCollected = 0;

    public bool CanAccessSC005 => sc002Complete && sc003Complete;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        tutorialComplete = PlayerPrefs.GetInt("TutorialComplete", 0) == 1;
        keysCollected = PlayerPrefs.GetInt("KeysCollected", 0);
        sc002Complete = PlayerPrefs.GetInt("SC002Complete", 0) == 1;
        sc003Complete = PlayerPrefs.GetInt("SC003Complete", 0) == 1;
    }

    public void SetTutorialComplete()
    {
        tutorialComplete = true;
        PlayerPrefs.SetInt("TutorialComplete", 1);
        PlayerPrefs.Save();
    }

    public void AddKey()
    {
        keysCollected++;
        PlayerPrefs.SetInt("KeysCollected", keysCollected);
        PlayerPrefs.Save();
    }

    public void SetSC002Complete()
    {
        sc002Complete = true;
        PlayerPrefs.SetInt("SC002Complete", 1);
        PlayerPrefs.Save();
    }

    public void SetSC003Complete()
    {
        sc003Complete = true;
        PlayerPrefs.SetInt("SC003Complete", 1);
        PlayerPrefs.Save();
    }

    public void ResetProgress()
    {
        boxPuzzleSolved = false;
        keyCollected = false;
        sc002Complete = false;
        sc003Complete = false;
        tutorialComplete = false;
        keysCollected = 0;
        PlayerPrefs.DeleteKey("TutorialComplete");
        PlayerPrefs.DeleteKey("KeysCollected");
        PlayerPrefs.DeleteKey("SC002Complete");
        PlayerPrefs.DeleteKey("SC003Complete");
        PlayerPrefs.Save();
    }
}