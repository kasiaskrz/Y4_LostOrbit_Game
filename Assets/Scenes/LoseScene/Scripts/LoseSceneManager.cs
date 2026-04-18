using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseSceneManager : MonoBehaviour
{
    public void OnRestartButton()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", "Room01");
        SceneManager.LoadScene(lastScene);
    }

    public void OnMenuButton()
    {
        // Find the SupabaseAuth instance (it's DontDestroyOnLoad)
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