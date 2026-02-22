using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        // Hide panel at start
        gameObject.SetActive(false);

        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        retryButton.onClick.AddListener(() =>
        {
            GameManager.Instance.RestartLevel();
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    // Called by GameManager
    public void ShowGameOver()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
}
