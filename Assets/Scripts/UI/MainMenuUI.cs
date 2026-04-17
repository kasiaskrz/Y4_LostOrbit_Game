using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button guestButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button waveModeButton;

    private void Awake()
    {
        loginButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginMenu");
        });

        guestButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.Room01);
        });

        waveModeButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.WaveMode);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });

        Time.timeScale = 1f;
    }
}