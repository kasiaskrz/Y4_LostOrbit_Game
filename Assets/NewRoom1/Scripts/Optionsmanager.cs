using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance { get; private set; }
    [HideInInspector] public float masterVolume = 1f;
    [HideInInspector] public float musicVolume = 1f;
    [HideInInspector] public float sfxVolume = 1f;

    public static KeyCode MoveForward = KeyCode.W;
    public static KeyCode MoveBack = KeyCode.S;
    public static KeyCode MoveLeft = KeyCode.A;
    public static KeyCode MoveRight = KeyCode.D;
    public static KeyCode Sprint = KeyCode.LeftShift;
    public static KeyCode Jump = KeyCode.Space;
    public static KeyCode Interact = KeyCode.E;
    public static KeyCode Reload = KeyCode.R;
    public static KeyCode Inventory = KeyCode.Tab;
    public static KeyCode Pause = KeyCode.Escape;
    public static KeyCode Inspect = KeyCode.F;
    public static KeyCode RotateLeft = KeyCode.T;
    public static KeyCode RotateRight = KeyCode.Y;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        LoadSettings();
    }

    public void ApplyVolume()
    {
        AudioListener.volume = masterVolume;
        foreach (var src in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (src.CompareTag("Music")) src.volume = musicVolume;
            else if (src.CompareTag("SFX")) src.volume = sfxVolume;
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("Key_MoveForward", (int)MoveForward);
        PlayerPrefs.SetInt("Key_MoveBack", (int)MoveBack);
        PlayerPrefs.SetInt("Key_MoveLeft", (int)MoveLeft);
        PlayerPrefs.SetInt("Key_MoveRight", (int)MoveRight);
        PlayerPrefs.SetInt("Key_Sprint", (int)Sprint);
        PlayerPrefs.SetInt("Key_Jump", (int)Jump);
        PlayerPrefs.SetInt("Key_Interact", (int)Interact);
        PlayerPrefs.SetInt("Key_Reload", (int)Reload);
        PlayerPrefs.SetInt("Key_Inventory", (int)Inventory);
        PlayerPrefs.SetInt("Key_Pause", (int)Pause);
        PlayerPrefs.SetInt("Key_Inspect", (int)Inspect);
        PlayerPrefs.SetInt("Key_RotateLeft", (int)RotateLeft);
        PlayerPrefs.SetInt("Key_RotateRight", (int)RotateRight);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        MoveForward = (KeyCode)PlayerPrefs.GetInt("Key_MoveForward", (int)KeyCode.W);
        MoveBack = (KeyCode)PlayerPrefs.GetInt("Key_MoveBack", (int)KeyCode.S);
        MoveLeft = (KeyCode)PlayerPrefs.GetInt("Key_MoveLeft", (int)KeyCode.A);
        MoveRight = (KeyCode)PlayerPrefs.GetInt("Key_MoveRight", (int)KeyCode.D);
        Sprint = (KeyCode)PlayerPrefs.GetInt("Key_Sprint", (int)KeyCode.LeftShift);
        Jump = (KeyCode)PlayerPrefs.GetInt("Key_Jump", (int)KeyCode.Space);
        Interact = (KeyCode)PlayerPrefs.GetInt("Key_Interact", (int)KeyCode.E);
        Reload = (KeyCode)PlayerPrefs.GetInt("Key_Reload", (int)KeyCode.R);
        Inventory = (KeyCode)PlayerPrefs.GetInt("Key_Inventory", (int)KeyCode.Tab);
        Pause = (KeyCode)PlayerPrefs.GetInt("Key_Pause", (int)KeyCode.Escape);
        Inspect = (KeyCode)PlayerPrefs.GetInt("Key_Inspect", (int)KeyCode.F);
        RotateLeft = (KeyCode)PlayerPrefs.GetInt("Key_RotateLeft", (int)KeyCode.T);
        RotateRight = (KeyCode)PlayerPrefs.GetInt("Key_RotateRight", (int)KeyCode.Y);
        ApplyVolume();
    }

    public void ResetToDefaults()
    {
        masterVolume = 1f; musicVolume = 1f; sfxVolume = 1f;
        MoveForward = KeyCode.W; MoveBack = KeyCode.S;
        MoveLeft = KeyCode.A; MoveRight = KeyCode.D;
        Sprint = KeyCode.LeftShift; Jump = KeyCode.Space;
        Interact = KeyCode.E; Reload = KeyCode.R;
        Inventory = KeyCode.Tab; Pause = KeyCode.Escape;
        Inspect = KeyCode.F;
        RotateLeft = KeyCode.T; RotateRight = KeyCode.Y;
        ApplyVolume();
        SaveSettings();
    }
}