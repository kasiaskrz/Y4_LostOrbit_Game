using UnityEngine;
using TMPro;

public class CrateInteraction : MonoBehaviour
{
    public TextMeshProUGUI interactText;   // optional – can be left empty

    private bool playerNear = false;
    private MovableBox moveScript;

    void Start()
    {
        moveScript = GetComponent<MovableBox>();

        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (moveScript == null)
            return;

        // if box finished moving, disable text & interaction
        if (moveScript.movementFinished)
        {
            if (interactText != null && interactText.gameObject.activeSelf)
                interactText.gameObject.SetActive(false);

            playerNear = false;
            return;
        }

        // Player presses E to move crate
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            moveScript.Activate();

            // hide text after first use
            if (interactText != null)
                interactText.gameObject.SetActive(false);

            playerNear = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (moveScript == null || moveScript.movementFinished)
            return;

        if (other.CompareTag("Player"))
        {
            playerNear = true;

            // only show text before first move
            if (interactText != null && !moveScript.hasBeenMovedOnce)
                interactText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;

            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }
}
