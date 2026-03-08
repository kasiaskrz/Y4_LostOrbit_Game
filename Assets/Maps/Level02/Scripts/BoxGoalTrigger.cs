using UnityEngine;

public class BoxGoalTrigger : MonoBehaviour
{
    public GameObject buttonToReveal;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("PushBox"))
        {
            activated = true;

            if (buttonToReveal != null)
            {
                buttonToReveal.SetActive(true);
            }

            Debug.Log("Box reached goal. Button revealed.");
        }
    }
}