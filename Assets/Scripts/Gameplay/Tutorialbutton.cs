using UnityEngine;

public class ButtonInteract : MonoBehaviour
{
    public TutorialManager tutorialManager;
    private bool inRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            tutorialManager.NotifyButtonPressed();
            Debug.Log("Button pressed!"); // Debug check
        }
    }
}
