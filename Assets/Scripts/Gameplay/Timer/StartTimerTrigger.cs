using UnityEngine;

public class StartTimerTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something touched the STOP trigger: " + other.name);

        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (GameTimer.Instance != null)
                GameTimer.Instance.StartTimer();

            Debug.Log("TIMER STARTED: Player left Tutorial Room");
        }
    }
}
