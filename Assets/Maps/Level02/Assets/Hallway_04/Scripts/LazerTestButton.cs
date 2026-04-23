using UnityEngine;
public class LaserTestButton : MonoBehaviour
{
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public LayerMask interactLayer = ~0;
    public LaserBounceTest linkedLaser;
    public bool oneUseOnly = true;
    private bool hasBeenUsed = false;
    private void Awake() { if (playerCamera == null) playerCamera = Camera.main; }
    private void Update()
    {
        if (oneUseOnly && hasBeenUsed) return;
        if (Input.GetKeyDown(OptionsManager.Interact)) TryInteract();
    }
    private void TryInteract()
    {
        if (playerCamera == null) return;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                PressButton();
    }
    private void PressButton()
    {
        if (linkedLaser == null) return;
        linkedLaser.ActivateLaser();
        hasBeenUsed = true;
    }
}