using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public string gameSceneName = "Room01";

    public void PlayAsGuest()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
