using UnityEngine;

public class LaserEmitterInteractable : MonoBehaviour
{
    public Transform pivot;                 // AntennaHead
    public float rotateSpeed = 80f;
    public float interactDistance = 2.5f;

    public KeyCode rotateLeftKey = KeyCode.T;
    public KeyCode rotateRightKey = KeyCode.Y;

    public Camera cam;

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (pivot == null || cam == null) return;

        if (!IsPlayerLookingAtThisBeacon()) return;

        if (Input.GetKey(rotateLeftKey))
            pivot.Rotate(0f, -rotateSpeed * Time.deltaTime, 0f, Space.Self);

        if (Input.GetKey(rotateRightKey))
            pivot.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.Self);
    }

    bool IsPlayerLookingAtThisBeacon()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            return hit.collider.GetComponentInParent<LaserEmitterInteractable>() == this;
        }

        return false;
    }
}
