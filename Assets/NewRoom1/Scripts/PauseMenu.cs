using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject helpPanel;
    public GameObject menuContainer;

    [Header("Buttons")]
    public Button resumeButton;
    public Button optionsButton;
    public Button helpButton;
    public Button quitButton;
    public Button helpBackButton;

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel == null) { Debug.LogError("[PauseMenu] pausePanel not assigned!"); return; }
        if (helpPanel == null) { Debug.LogError("[PauseMenu] helpPanel not assigned!"); return; }
        if (menuContainer == null) { Debug.LogError("[PauseMenu] menuContainer not assigned!"); return; }

        pausePanel.SetActive(false);
        helpPanel.SetActive(false);

        if (resumeButton) resumeButton.onClick.AddListener(Resume);
        if (optionsButton) optionsButton.onClick.AddListener(Options);
        if (helpButton) helpButton.onClick.AddListener(OpenHelp);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
        if (helpBackButton) helpBackButton.onClick.AddListener(CloseHelp);
    }

    void Update()
    {
        if (NotePickup.IsOpen) return;
        if (WirePuzzle.IsOpen) return;
        if (LevelComplete.IsOpen) return; // block pause menu when level complete is open

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (helpPanel != null && helpPanel.activeSelf)
                CloseHelp();
            else if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        menuContainer.SetActive(true);
        helpPanel.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        helpPanel.SetActive(false);
        menuContainer.SetActive(true);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Options()
    {
        Debug.Log("[PauseMenu] Options clicked - not implemented yet");
    }

    void OpenHelp()
    {
        menuContainer.SetActive(false);
        helpPanel.SetActive(true);
    }

    void CloseHelp()
    {
        helpPanel.SetActive(false);
        menuContainer.SetActive(true);
    }

    void QuitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("[PauseMenu] Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}