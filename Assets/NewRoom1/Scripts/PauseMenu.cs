using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject helpPanel;
    public GameObject optionsPanel;
    public GameObject menuContainer;

    [Header("Buttons")]
    public Button resumeButton;
    public Button optionsButton;
    public Button helpButton;
    public Button quitButton;
    public Button helpBackButton;

    private bool isPaused = false;
    public static bool EscConsumed = false;


    void Start()
    {
        if (pausePanel == null) { Debug.LogError("[PauseMenu] pausePanel not assigned!"); return; }
        if (helpPanel == null) { Debug.LogError("[PauseMenu] helpPanel not assigned!"); return; }
        if (menuContainer == null) { Debug.LogError("[PauseMenu] menuContainer not assigned!"); return; }

        pausePanel.SetActive(false);
        helpPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);

        if (resumeButton) resumeButton.onClick.AddListener(Resume);
        if (optionsButton) optionsButton.onClick.AddListener(OpenOptions);
        if (helpButton) helpButton.onClick.AddListener(OpenHelp);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
        if (helpBackButton) helpBackButton.onClick.AddListener(CloseHelp);
    }

    void Update()
    {
        if (EscConsumed) { EscConsumed = false; return; } 
        if (NotePickup.IsOpen) return;
        if (WirePuzzle.IsOpen) return;
        if (LevelComplete.IsOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (helpPanel != null && helpPanel.activeSelf)
                CloseHelp();
            else if (optionsPanel != null && optionsPanel.activeSelf)
                CloseOptions();
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
        if (optionsPanel) optionsPanel.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        helpPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        menuContainer.SetActive(true);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OpenOptions()
    {
        menuContainer.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
        menuContainer.SetActive(true);
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
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}