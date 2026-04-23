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
        if (interactText != null) interactText.gameObject.SetActive(false);
    }
    void Update()
    {
        if (moveScript == null) return;
        if (moveScript.movementFinished) { if (interactText != null) interactText.gameObject.SetActive(false); playerNear = false; return; }
        if (interactText != null && interactText.gameObject.activeSelf)
            interactText.text = $"Press [{OptionsManager.Interact}] to push";
        if (playerNear && Input.GetKeyDown(OptionsManager.Interact))
        { moveScript.Activate(); if (interactText != null) interactText.gameObject.SetActive(false); playerNear = false; }
    }
    private void OnTriggerEnter(Collider other)
    { if (moveScript == null || moveScript.movementFinished) return; if (other.CompareTag("Player")) { playerNear = true; if (interactText != null && !moveScript.hasBeenMovedOnce) interactText.gameObject.SetActive(true); } }
    private void OnTriggerExit(Collider other)
    { if (other.CompareTag("Player")) { playerNear = false; if (interactText != null) interactText.gameObject.SetActive(false); } }
}