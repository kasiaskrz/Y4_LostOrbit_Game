using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string menuSceneName = "MainMenuScene";

    public void GoToMenu()
    {
        SupabaseAuth auth = FindFirstObjectByType<SupabaseAuth>();
        if (auth != null)
        {
            auth.accessToken = "";
            auth.userId = "";
        }

        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(menuSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}