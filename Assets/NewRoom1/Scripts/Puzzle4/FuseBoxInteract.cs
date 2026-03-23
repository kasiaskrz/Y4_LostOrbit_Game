using UnityEngine;

public class FuseBoxInteract : MonoBehaviour, IInteractable
{
    public WirePuzzle wirePuzzle;
    // public DoorController door;

    private bool puzzleSolved = false;

    public string PromptText => puzzleSolved ? "" : "Open Fuse Box";

    void Start()
    {
        if (wirePuzzle != null)
        {
            wirePuzzle.OnSolved += HandleSolved;
            wirePuzzle.puzzlePanel.SetActive(false);
        }
    }

    public void Interact()
    {
        if (puzzleSolved) return;

        Generator gen = FindFirstObjectByType<Generator>();
        if (gen == null || !gen.isPowered)
        {
            Debug.Log("Generator not powered yet.");
            return;
        }

        wirePuzzle.puzzlePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void HandleSolved()
    {
        puzzleSolved = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Puzzle solved! Door would open here.");
        // door.Open();
    }
}