using UnityEngine;

public class BarricadeTrigger : MonoBehaviour
{
    public BarricadeSequence sequence;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (sequence != null)
        {
            sequence.StartSequence();
        }
    }
}