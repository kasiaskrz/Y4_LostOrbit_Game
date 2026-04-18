using UnityEngine;

/// <summary>
/// Attach to the Button GameObject in LVL02_Sniperalley.
/// Disabled by default - PushBox enables it once the box is fully pushed.
/// Shows "Shoot here" popup only when the player's crosshair raycast hits this button.
/// Hides the "Find the button" popup once the player finds it.
/// Call OnButtonShot() from ShootableButton when the button is activated.
/// </summary>
public class ButtonLookHint : MonoBehaviour
{
    [Header("Popups")]
    public GameObject shootHerePopup;   // "Shoot here" panel
    public GameObject findButtonPopup;  // "Find the button" panel - hides when player finds button

    [Header("Raycast Settings")]
    public float maxDistance = 50f;
    public LayerMask layerMask = ~0;
    public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

    private Camera playerCam;
    private bool buttonShot = false;

    void OnEnable()
    {
        // Get camera each time in case it wasn't ready at Start
        if (playerCam == null)
            playerCam = Camera.main;
    }

    void Update()
    {
        if (buttonShot || playerCam == null) return;

        bool lookingAtButton = false;

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask, triggerInteraction))
        {
            if (hit.collider.gameObject == gameObject ||
                hit.collider.transform.IsChildOf(transform))
            {
                lookingAtButton = true;
            }
        }

        if (lookingAtButton)
        {
            // Player found the button - hide "find it" hint
            if (findButtonPopup != null)
                findButtonPopup.SetActive(false);

            // Show "shoot here"
            if (shootHerePopup != null)
                shootHerePopup.SetActive(true);
        }
        else
        {
            if (shootHerePopup != null)
                shootHerePopup.SetActive(false);
        }
    }

    // Called from ShootableButton.cs when the button is activated
    public void OnButtonShot()
    {
        buttonShot = true;

        if (shootHerePopup != null)
            shootHerePopup.SetActive(false);

        if (findButtonPopup != null)
            findButtonPopup.SetActive(false);
    }
}