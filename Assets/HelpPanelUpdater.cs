using UnityEngine;
using TMPro;

public class HelpPanelUpdater : MonoBehaviour
{
    [Header("Assign each text label from the HelpPanel hierarchy")]
    public TextMeshProUGUI moveText;
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI sprintText;
    public TextMeshProUGUI reloadText;
    public TextMeshProUGUI inspectText;
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI pauseText;
    public TextMeshProUGUI inventoryText;
    public TextMeshProUGUI rotateLeftText;
    public TextMeshProUGUI rotateRightText;

    void OnEnable()
    {
        UpdateLabels();
    }

    public void UpdateLabels()
    {
        if (moveText) moveText.text = $"{OptionsManager.MoveForward}/{OptionsManager.MoveLeft}/{OptionsManager.MoveBack}/{OptionsManager.MoveRight}:    Move";
        if (jumpText) jumpText.text = $"{OptionsManager.Jump}:    Jump";
        if (sprintText) sprintText.text = $"{OptionsManager.Sprint}:    Sprint";
        if (reloadText) reloadText.text = $"{OptionsManager.Reload}:    Reload";
        if (inspectText) inspectText.text = $"{OptionsManager.Inspect}:    Inspect";
        if (interactText) interactText.text = $"{OptionsManager.Interact}:    Interact";
        if (pauseText) pauseText.text = $"{OptionsManager.Pause}:    Pause / Resume";
        if (inventoryText) inventoryText.text = $"{OptionsManager.Inventory}:    Open/Close Inventory";
        if (rotateLeftText) rotateLeftText.text = $"{OptionsManager.RotateLeft}:    Rotate Left";
        if (rotateRightText) rotateRightText.text = $"{OptionsManager.RotateRight}:    Rotate Right";
    }
}