using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button guestButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button waveModeButton;

    [Header("Music")]
    public AudioClip menuMusic;
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    private void Awake()
    {
        if (menuMusic != null)
        {
            AudioSource musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        loginButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginMenu");
        });

        guestButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.SC001);
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