using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseSceneManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip deathSound;
    [Range(0f, 5f)]
    public float deathVolume = 1f;

    public AudioClip loopSound;
    [Range(0f, 5f)]
    public float loopVolume = 0.5f;

    private void Start()
    {
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, Vector3.zero, deathVolume);

        if (loopSound != null)
            StartCoroutine(PlayLoopAfterDeathSound());
    }

    private System.Collections.IEnumerator PlayLoopAfterDeathSound()
    {
        // Wait for the death sound to finish
        yield return new WaitForSeconds(deathSound.length);

        AudioSource loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.clip = loopSound;
        loopSource.loop = true;
        loopSource.volume = loopVolume;
        loopSource.Play();
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