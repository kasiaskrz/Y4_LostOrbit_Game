using UnityEngine;

public class CursorUiToggle : MonoBehaviour
{
    [Header("Drag your look script here")]
    public MonoBehaviour lookScript; // e.g., FirstPersonLook

    bool uiOpen = false;

    void Start()
    {
        SetGameplayMode();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiOpen = !uiOpen;

            if (uiOpen) SetUIMode();
            else SetGameplayMode();
        }
    }

    void SetUIMode()
    {
        if (lookScript != null) lookScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void SetGameplayMode()
    {
        if (lookScript != null) lookScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
