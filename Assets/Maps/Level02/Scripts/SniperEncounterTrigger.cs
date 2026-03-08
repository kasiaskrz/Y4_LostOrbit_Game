using UnityEngine;

public class SniperEncounterTrigger : MonoBehaviour
{
    public SniperEncounter encounter;
    public string playerTag = "Player";
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag(playerTag))
        {
            triggered = true;

            if (encounter != null)
            {
                encounter.StartEncounter();
            }
        }
    }
}