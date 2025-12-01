using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        // Hide this panel at the start
        gameObject.SetActive(false);

        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        retryButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.SC001);   // your gameplay scene
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    public void ShowGameOver()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;   // optional: pause game
    }
}
