using UnityEngine;

public class SC001Exit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameProgress.Instance != null)
            GameProgress.Instance.tutorialComplete = true;
    }
}