using UnityEngine;
using TMPro;

public class AreaPopupTrigger : MonoBehaviour
{
    public GameObject popup;

    [Header("Optional — if the popup has a TMP text, it will show the current interact key")]
    public TextMeshProUGUI popupText;

    [Header("Text template — use {KEY} where the key should appear")]
    public string template = "Press [{KEY}] to interact";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (popup != null)
        {
            popup.SetActive(true);
            UpdateText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (popup != null) popup.SetActive(false);
    }

    void Update()
    {
        // Keep text fresh if player is inside and rebinds mid-game
        if (popup != null && popup.activeSelf)
            UpdateText();
    }

    void UpdateText()
    {
        if (popupText != null)
            popupText.text = template.Replace("{KEY}", OptionsManager.Interact.ToString());
    }
}