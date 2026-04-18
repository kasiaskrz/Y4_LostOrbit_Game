using UnityEngine;

/// <summary>
/// Place on an empty GameObject with a Box Collider (Is Trigger checked) near the boxtarget.
/// Shows "Find the button" popup when player enters the zone.
/// Disappears permanently once the player looks at the button.
/// </summary>
public class FindButtonHint : MonoBehaviour
{
    public GameObject findButtonPopup;  // assign FindButtonPopup panel in Inspector
    public Transform buttonTransform;   // assign the Button GameObject in Inspector
    public float maxDistance = 100f;    // how far the raycast checks

    private Camera playerCam;
    private bool playerInZone = false;
    private bool done = false;

    void Start()
    {
        playerCam = Camera.main;
        if (findButtonPopup != null)
            findButtonPopup.SetActive(false);
    }

    void Update()
    {
        if (done || !playerInZone || playerCam == null) return;

        // Check if player is looking at the button
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (buttonTransform != null &&
                (hit.collider.gameObject == buttonTransform.gameObject ||
                 hit.collider.transform.IsChildOf(buttonTransform)))
            {
                // Player found the button - hide forever
                done = true;
                if (findButtonPopup != null)
                    findButtonPopup.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (done) return;
        if (!other.CompareTag("Player")) return;
        playerInZone = true;
        if (findButtonPopup != null)
            findButtonPopup.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = false;
        if (!done && findButtonPopup != null)
            findButtonPopup.SetActive(false);
    }
}