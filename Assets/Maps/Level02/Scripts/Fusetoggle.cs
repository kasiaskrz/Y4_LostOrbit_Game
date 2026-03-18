using UnityEngine;

public class FuseToggle : MonoBehaviour
{
    [Header("Interaction")]
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactLayer = ~0;

    [Header("Fuse Visual")]
    public GameObject fuseObject;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                ToggleFuse();
            }
        }
    }

    private void ToggleFuse()
    {
        if (fuseObject == null) return;

        fuseObject.SetActive(!fuseObject.activeSelf);

        Debug.Log("Fuse toggled.");
    }
}