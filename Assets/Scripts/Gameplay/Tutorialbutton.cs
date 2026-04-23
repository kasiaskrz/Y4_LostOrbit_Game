using UnityEngine;
public class ButtonInteract : MonoBehaviour
{
    private bool inRange = false;
    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) inRange = true; }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) inRange = false; }
    void Update()
    {
        if (inRange && Input.GetKeyDown(OptionsManager.Interact))
            Debug.Log("Button pressed!");
    }
}