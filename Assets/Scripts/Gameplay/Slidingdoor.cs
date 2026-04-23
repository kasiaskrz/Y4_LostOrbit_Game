using UnityEngine;
public class SlidingDoor : MonoBehaviour
{
    public Transform doorPanel;
    public Vector3 openOffset = new Vector3(0, 2.5f, 0);
    public float speed = 3f;
    public float closeDelay = 2f;
    public string playerTag = "Player";
    private Vector3 closedPos, openPos;
    private bool isOpen = false;
    private float closeTimer = 0f;
    private bool playerInRange = false;
    private void Start()
    {
        if (doorPanel == null) { enabled = false; return; }
        closedPos = doorPanel.localPosition;
        openPos = closedPos + openOffset;
    }
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(OptionsManager.Interact)) { isOpen = true; closeTimer = closeDelay; }
        doorPanel.localPosition = Vector3.Lerp(doorPanel.localPosition, isOpen ? openPos : closedPos, Time.deltaTime * speed);
        if (isOpen) { closeTimer -= Time.deltaTime; if (closeTimer <= 0f) isOpen = false; }
    }
    private void OnTriggerEnter(Collider other) { if (other.CompareTag(playerTag)) playerInRange = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag(playerTag)) playerInRange = false; }
}