using UnityEngine;

public class FuseBoxInteract : MonoBehaviour, IInteractable
{
    public WirePuzzle wirePuzzle;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip solvedSound;
    public SlidingDoorLevel1 door;

    private bool puzzleSolved = false;
    private Generator _generator;

    public string PromptText
    {
        get
        {
            if (puzzleSolved) return "";
            if (_generator != null && _generator.isPowered) return "Open Fuse Box";
            return "Locked - need power";
        }
    }

    void Start()
    {
        _generator = FindFirstObjectByType<Generator>();

        if (wirePuzzle != null)
        {
            wirePuzzle.OnSolved += HandleSolved;
            wirePuzzle.puzzlePanel.SetActive(false);
        }
    }

    public void Interact()
    {
        if (puzzleSolved) return;

        if (_generator == null || !_generator.isPowered)
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

        if (audioSource != null && solvedSound != null)
            audioSource.PlayOneShot(solvedSound);

        if (door != null)
            door.Unlock();
    }
}