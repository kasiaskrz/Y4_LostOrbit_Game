using UnityEngine;

public class PlayerInteract2 : MonoBehaviour
{
    [Header("Interaction")]
    public float interactDistance = 2f;
    public KeyCode interactKey = KeyCode.E;

    [Header("References")]
    public Camera playerCamera;

    [Header("UI")]
    public GameObject interactPrompt; // show/hide only (simple prototype)

    private IInteractable current;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        current = null;
        bool canInteract = false;

        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                current = hit.collider.GetComponentInParent<IInteractable>();
                if (current != null)
                {
                    canInteract = true;

                    if (Input.GetKeyDown(interactKey))
                        current.Interact();
                }
            }
        }

        if (interactPrompt != null)
            interactPrompt.SetActive(canInteract);
    }
}
