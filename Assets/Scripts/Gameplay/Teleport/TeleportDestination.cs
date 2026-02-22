using UnityEngine;

public class TeleportDestination : MonoBehaviour
{
    [Tooltip("ID that TeleportOnTrigger will look for.")]
    public string destinationID = "DoorFromMainHall";

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
