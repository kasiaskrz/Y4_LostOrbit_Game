using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip loopSound;
    [Range(0f, 5f)]
    public float loopVolume = 0.5f;

    private AudioSource loopSource;

    private void Start()
    {
        // unlock mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (loopSound != null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.clip = loopSound;
            loopSource.loop = true;
            loopSource.volume = loopVolume;
            loopSource.Play();
        }
    }

    public void OnRestartButton()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", "Room01");
        SceneManager.LoadScene(lastScene);
    }

    public void OnMenuButton()
    {
        SupabaseAuth auth = FindFirstObjectByType<SupabaseAuth>();
        if (auth != null)
        {
            auth.accessToken = "";
            auth.userId = "";
        }

        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}