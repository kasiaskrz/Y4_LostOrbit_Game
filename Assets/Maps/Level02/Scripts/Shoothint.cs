using UnityEngine;

public class ShootHint : MonoBehaviour
{
    public GameObject shootHerePopup;
    public float maxDistance = 100f;

    private Camera playerCam;
    private bool done = false;

    void Start()
    {
        playerCam = Camera.main;
        if (shootHerePopup != null)
            shootHerePopup.SetActive(false);
    }

    void Update()
    {
        if (done || playerCam == null) return;

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        bool lookingAtButton = false;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.gameObject == gameObject ||
                hit.collider.transform.IsChildOf(transform))
            {
                lookingAtButton = true;
            }
        }

        if (shootHerePopup != null)
            shootHerePopup.SetActive(lookingAtButton);
    }

    // Called when the GameObject is destroyed - hides the popup
    void OnDestroy()
    {
        done = true;
        if (shootHerePopup != null)
            shootHerePopup.SetActive(false);
    }
}