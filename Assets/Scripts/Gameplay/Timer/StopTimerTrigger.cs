using UnityEngine;

public class StopTimerTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (GameTimer.Instance != null)
            {
                GameTimer.Instance.StopTimer();
                Debug.Log("ROOM COMPLETED in: " + Mathf.CeilToInt(GameTimer.Instance.timeRemaining));
            }

            Debug.Log("You may now open the door to proceed.");
        }
    }
}
