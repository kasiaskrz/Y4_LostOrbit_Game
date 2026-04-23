using UnityEngine;

public class LaserEmitterInteractable : MonoBehaviour, IInteractable
{
    public Transform pivot;
    public float rotateSpeed = 80f;
    public float interactDistance = 2.5f;
    public Camera cam;

    public string PromptText => $"[{OptionsManager.RotateLeft}/{OptionsManager.RotateRight}] Rotate";

    public void Interact() { }

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (pivot == null || cam == null) return;
        if (!IsPlayerLookingAtThisBeacon()) return;

        if (Input.GetKey(OptionsManager.RotateLeft))
            pivot.Rotate(0f, -rotateSpeed * Time.deltaTime, 0f, Space.Self);

        if (Input.GetKey(OptionsManager.RotateRight))
            pivot.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.Self);
    }

    bool IsPlayerLookingAtThisBeacon()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            return hit.collider.GetComponentInParent<LaserEmitterInteractable>() == this;
        return false;
    }
}