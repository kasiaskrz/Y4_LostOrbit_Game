using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {

        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        Debug.Log("Level Completed!");

        if (LevelComplete.Instance != null)
            LevelComplete.Instance.TriggerLevelComplete();
        else
            Debug.LogError("LevelComplete.Instance is null!");
    }
}
