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
        Debug.Log("[PauseMenu] Start() ran on: " + gameObject.name);

        // Wire up buttons FIRST
        if (resumeButton)   resumeButton.onClick.AddListener(Resume);
        if (optionsButton)  optionsButton.onClick.AddListener(OpenOptions);
        if (helpButton)     helpButton.onClick.AddListener(OpenHelp);
        if (quitButton)     quitButton.onClick.AddListener(QuitGame);
        if (helpBackButton) helpBackButton.onClick.AddListener(CloseHelp);

        Debug.Log("[PauseMenu] resumeButton null? " + (resumeButton == null));
        Debug.Log("[PauseMenu] optionsButton null? " + (optionsButton == null));

        if (pausePanel == null)    Debug.LogError("[PauseMenu] pausePanel not assigned!");
        if (helpPanel == null)     Debug.LogError("[PauseMenu] helpPanel not assigned!");
        if (menuContainer == null) Debug.LogError("[PauseMenu] menuContainer not assigned!");

        if (pausePanel)    pausePanel.SetActive(false);
        if (helpPanel)     helpPanel.SetActive(false);
        if (menuContainer) menuContainer.SetActive(true);
        if (optionsPanel)  optionsPanel.SetActive(false);
    }

    void Update()
    {
        if (EscConsumed) { EscConsumed = false; return; }
        if (NotePickup.IsOpen)    return;
        if (WirePuzzle.IsOpen)    return;
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
        if (pausePanel == null || menuContainer == null) return;
        isPaused = true;
        pausePanel.SetActive(true);
        menuContainer.SetActive(true);
        if (helpPanel)    helpPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        if (pausePanel == null || menuContainer == null) return;
        isPaused = false;
        pausePanel.SetActive(false);
        if (helpPanel)    helpPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        menuContainer.SetActive(true);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OpenOptions()
    {
        if (menuContainer) menuContainer.SetActive(false);
        if (optionsPanel)  optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel)  optionsPanel.SetActive(false);
        if (menuContainer) menuContainer.SetActive(true);
    }

    void OpenHelp()
    {
        if (menuContainer) menuContainer.SetActive(false);
        if (helpPanel)     helpPanel.SetActive(true);
    }

    void CloseHelp()
    {
        if (helpPanel)     helpPanel.SetActive(false);
        if (menuContainer) menuContainer.SetActive(true);
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