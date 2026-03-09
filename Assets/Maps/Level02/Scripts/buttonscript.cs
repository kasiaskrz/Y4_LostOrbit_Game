using UnityEngine;

public class LaserButton : MonoBehaviour
{
    public LaserGate linkedGate;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;
    private bool hasBeenPressed = false;

    void Update()
    {
        if (playerInRange && !hasBeenPressed && Input.GetKeyDown(interactKey))
        {
            if (linkedGate != null)
            {
                linkedGate.DisableLaser();
                hasBeenPressed = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}