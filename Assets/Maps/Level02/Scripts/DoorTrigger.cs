using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Doors Controlled By This Trigger")]
    public AutomaticDoor[] linkedDoors;

    [Header("Detection")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        foreach (AutomaticDoor door in linkedDoors)
        {
            if (door != null)
                door.OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        foreach (AutomaticDoor door in linkedDoors)
        {
            if (door != null)
                door.CloseDoor();
        }
    }
}