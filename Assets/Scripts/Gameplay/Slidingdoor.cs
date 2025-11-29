using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Door Panel")]
    public Transform doorPanel;                // The sliding mesh
    public Vector3 openOffset = new Vector3(0, 2.5f, 0);
    public float speed = 3f;

    [Header("Auto Close")]
    public float closeDelay = 2f;             // Time before closing

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;   // Key to press at the button
    public string playerTag = "Player";       // Tag of the player

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;
    private float closeTimer = 0f;
    private bool playerInRange = false;

    private void Start()
    {
        if (doorPanel == null)
        {
            Debug.LogError("SlidingDoor: Door Panel is not assigned!");
            enabled = false;
            return;
        }

        closedPos = doorPanel.localPosition;
        openPos = closedPos + openOffset;
    }

    private void Update()
    {
        // Handle interaction
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            // Open (or re-open) the door
            isOpen = true;
            closeTimer = closeDelay;
        }

        // Door movement
        if (isOpen)
        {
            // Open the door
            doorPanel.localPosition = Vector3.Lerp(
                doorPanel.localPosition,
                openPos,
                Time.deltaTime * speed
            );

            // Countdown to auto-close
            closeTimer -= Time.deltaTime;
            if (closeTimer <= 0f)
                isOpen = false;
        }
        else
        {
            // Close the door
            doorPanel.localPosition = Vector3.Lerp(
                doorPanel.localPosition,
                closedPos,
                Time.deltaTime * speed
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }
}
