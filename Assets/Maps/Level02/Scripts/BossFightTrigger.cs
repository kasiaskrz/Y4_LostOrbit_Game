using UnityEngine;
using UnityEngine.SceneManagement;

public class BossFightTrigger : MonoBehaviour
{
    [Header("Scene")]
    public string bossSceneName = "BossFight";

    [Header("Trigger Settings")]
    public string playerTag = "Player";
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce)
            return;

        if (!other.CompareTag(playerTag))
            return;

        hasTriggered = true;
        SceneManager.LoadScene(bossSceneName);
    }
}