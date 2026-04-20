using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelComplete : MonoBehaviour
{
    public static LevelComplete Instance { get; private set; }
    public static bool IsOpen { get; private set; }

    [Header("UI")]
    public GameObject levelCompletePanel;
    public Button continueButton;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelText;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 0.8f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        levelCompletePanel.SetActive(false);
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    void Start()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(LoadNextScene);
    }

    public void TriggerLevelComplete()
    {
        StartCoroutine(FadeAndShow());
    }

    IEnumerator FadeAndShow()
    {
        IsOpen = true;

        LevelRunReporter reporter = FindFirstObjectByType<LevelRunReporter>();
        if (reporter != null)
            reporter.StopTimer();

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // fade to black
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // update UI texts
        if (reporter != null)
        {
            int minutes = Mathf.FloorToInt(reporter.elapsed / 60f);
            int seconds = Mathf.FloorToInt(reporter.elapsed % 60f);

            if (timeText != null)
                timeText.text = $"Time: {minutes:00}:{seconds:00}";

            if (levelText != null)
                levelText.text = $"Level {reporter.levelNumber}";
        }

        HUDController hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
            hud.gameObject.SetActive(false);

        levelCompletePanel.SetActive(true);
    }

    void LoadNextScene()
    {
        IsOpen = false;
        Time.timeScale = 1f;
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }
}