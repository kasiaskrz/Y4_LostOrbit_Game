using UnityEngine;
using TMPro;

public class CrateInteraction : MonoBehaviour
{
    public TextMeshProUGUI interactText;

    private bool playerNear = false;
    private MovableBox moveScript;

    void Start()
    {
        moveScript = GetComponent<MovableBox>();
    }

    void Update()
    {
        // If moved once, force-hide forever
        if (moveScript != null && moveScript.hasBeenMoved)
        {
            if (interactText.gameObject.activeSelf)
                interactText.gameObject.SetActive(false);

            playerNear = false;
            return;
        }

        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interacted with crate!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !moveScript.hasBeenMoved)
        {
            playerNear = true;
            interactText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            interactText.gameObject.SetActive(false);
        }
    }
}
