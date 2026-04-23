using UnityEngine;
using TMPro;

public class ButtonInteractL02 : MonoBehaviour
{
    [Header("Optional popup text to show key hint")]
    public TextMeshProUGUI popupText;

    private bool inRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            UpdatePopupText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            inRange = false;
    }

    void Update()
    {
        if (!inRange) return;

        // Keep popup text fresh in case keybind changed
        UpdatePopupText();

        if (Input.GetKeyDown(OptionsManager.Interact))
            Debug.Log("Button pressed!");
    }

    void UpdatePopupText()
    {
        if (popupText != null)
            popupText.text = $"Press [{OptionsManager.Interact}] to activate";
    }
}