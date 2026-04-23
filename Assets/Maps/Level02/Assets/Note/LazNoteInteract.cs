using UnityEngine;
public class LazNoteInteract : MonoBehaviour
{
    public GameObject noteUIRoot;
    public string playerTag = "Player";
    private bool playerInRange = false;
    private bool isReading = false;
    void Start() { if (noteUIRoot != null) noteUIRoot.SetActive(false); }
    void Update()
    {
        if (!playerInRange) return;
        if (Input.GetKeyDown(OptionsManager.Interact))
        {
            if (!isReading) OpenNote();
            else CloseNote();
        }
    }
    void OpenNote()
    {
        isReading = true;
        if (noteUIRoot != null) noteUIRoot.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void CloseNote()
    {
        isReading = false;
        if (noteUIRoot != null) noteUIRoot.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnTriggerEnter(Collider other) { if (other.CompareTag(playerTag)) playerInRange = true; }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag)) { playerInRange = false; if (isReading) CloseNote(); }
    }
}