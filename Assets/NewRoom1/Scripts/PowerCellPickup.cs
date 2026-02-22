using UnityEngine;

public class PowerCellPickup : MonoBehaviour
{
    public Camera cam;
    public Transform holdPoint;
    public float pickupDistance = 2f;
    public KeyCode pickupKey = KeyCode.E;

    public PowerCell heldCell;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldCell == null)
                TryPickup();
        }

        if (heldCell != null)
        {
            heldCell.transform.position = holdPoint.position;
            heldCell.transform.rotation = holdPoint.rotation;
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            PowerCell cell = hit.collider.GetComponent<PowerCell>();
            if (cell != null)
            {
                heldCell = cell;

                // Disable collider so it doesn't interfere
                Collider col = cell.GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }
    }
}
