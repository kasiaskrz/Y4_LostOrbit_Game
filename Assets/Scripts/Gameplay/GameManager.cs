using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Assign the GameOverPanel object here!")]
    public GameOverUI gameOverUI;   // Drag GameOverPanel here in Inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // CALLED by EnemyHealth → Die()
    public void WinGame()
    {
        Debug.Log("Enemy defeated → Showing Game Over UI");
        gameOverUI.ShowGameOver();  // SHOW UI
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
