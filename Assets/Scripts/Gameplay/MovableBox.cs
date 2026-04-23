using UnityEngine;
public class MovableBox : MonoBehaviour
{
    public enum BoxMode { SingleOffset, MultiTarget }
    public BoxMode mode = BoxMode.SingleOffset;
    public float moveSpeed = 2f;
    public float interactRange = 2f;
    public Vector3 targetOffset = new Vector3(-2f, 0f, -2f);
    public Transform[] goalSpots;
    public GameObject keyObject;
    [HideInInspector] public bool hasBeenMovedOnce = false;
    [HideInInspector] public bool movementFinished = false;
    private Transform player;
    private Vector3 startPos, singleTargetPos;
    private int currentTargetIndex = 0;
    private bool activated = false;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position; targetOffset.y = 0;
        singleTargetPos = startPos + targetOffset;
        if (keyObject != null) keyObject.SetActive(false);
    }
    void Update()
    {
        if (movementFinished || player == null) return;
        if (!activated && Vector3.Distance(player.position, transform.position) < interactRange && Input.GetKeyDown(OptionsManager.Interact))
            Activate();
        if (activated) MoveBox();
    }
    public void Activate() { if (!hasBeenMovedOnce) hasBeenMovedOnce = true; if (movementFinished) return; activated = true; }
    private void MoveBox()
    {
        Vector3 targetPos = mode == BoxMode.SingleOffset ? singleTargetPos : (goalSpots != null && goalSpots.Length > 0 ? goalSpots[currentTargetIndex].position : singleTargetPos);
        targetPos.y = startPos.y;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            activated = false;
            if (mode == BoxMode.SingleOffset) { movementFinished = true; }
            else { currentTargetIndex++; if (currentTargetIndex >= goalSpots.Length) { movementFinished = true; if (keyObject != null) keyObject.SetActive(true); } }
        }
    }
}