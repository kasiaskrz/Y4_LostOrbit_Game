using UnityEngine;

public class Door : MonoBehaviour
{
    public void OpenDoor()
    {
        // For now, just hide it. Later you can animate it.
        gameObject.SetActive(false);
    }
}
