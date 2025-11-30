using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    public MonoBehaviour playerController;
    public TMP_Text tutorialText;
    public GameObject buttonObject;

    private bool hasMoved = false;
    private bool crateMoved = false;   // 👈 NEW
    private bool buttonPressed = false;

    void Start()
    {
        SetPlayerCanMove(false);

        if (buttonObject != null)
            buttonObject.SetActive(false);

        StartCoroutine(TutorialFlow());
    }

    private System.Collections.IEnumerator TutorialFlow()
    {
        // Step 1: Intro
        SetText("Welcome to Lost orbit.\nThis is your training room.");
        yield return new WaitForSeconds(3f);

        // Step 2: Enable movement
        SetText("Use WASD or Arrow Keys to move.");
        SetPlayerCanMove(true);

        hasMoved = false;
        while (!hasMoved)
        {
            if (PlayerHasTriedToMove())
                hasMoved = true;

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // ⭐ NEW STEP: Push the crate with E
        crateMoved = false;
        SetText("Good.\nNow push the crate using E.");

        while (!crateMoved)
            yield return null;

        yield return new WaitForSeconds(1f);

        // Step 3: Show button tutorial
        if (buttonObject != null)
            buttonObject.SetActive(true);

        SetText("Nice!\nNow go to the button and press E to interact.");

        buttonPressed = false;
        while (!buttonPressed)
            yield return null;

        // Step 4: End tutorial
        SetText("Tutorial complete. Leaving the room...");
    }

    // 👈 NEW — crate script calls this
    public void NotifyCrateMoved()
    {
        crateMoved = true;
    }

    public void NotifyButtonPressed()
    {
        buttonPressed = true;
    }

    private void SetPlayerCanMove(bool value)
    {
        if (playerController == null) return;

        var type = playerController.GetType();
        var field = type.GetField("canMove");
        if (field != null)
        {
            field.SetValue(playerController, value);
        }
    }

    private bool PlayerHasTriedToMove()
    {
        if (playerController == null) return false;

        var type = playerController.GetType();
        var method = type.GetMethod("HasTriedToMove");
        if (method != null)
        {
            return (bool)method.Invoke(playerController, null);
        }

        return false;
    }

    private void SetText(string msg)
    {
        if (tutorialText != null)
            tutorialText.text = msg;
    }
}
