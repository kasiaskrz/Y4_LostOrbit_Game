using UnityEngine;

public class MovableBox : MonoBehaviour
{
    public enum BoxMode
    {
        SingleOffset,
        MultiTarget
    }

    [Header("Mode")]
    public BoxMode mode = BoxMode.SingleOffset;

    [Header("Shared Settings")]
    public float moveSpeed = 2f;
    public float interactRange = 2f;

    [Header("Single Offset Mode")]
    public Vector3 targetOffset = new Vector3(-2f, 0f, -2f);

    [Header("Multi Target Mode")]
    public Transform[] goalSpots;
    public GameObject keyObject;

    [HideInInspector] public bool hasBeenMovedOnce = false;
    [HideInInspector] public bool movementFinished = false;

    private Transform player;
    private Vector3 startPos;
    private Vector3 singleTargetPos;
    private int currentTargetIndex = 0;
    private bool activated = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position;
        targetOffset.y = 0;
        singleTargetPos = startPos + targetOffset;

        if (keyObject != null)
            keyObject.SetActive(false);
    }

    void Update()
    {
        if (movementFinished || player == null) return;

        if (!activated &&
            Vector3.Distance(player.position, transform.position) < interactRange &&
            Input.GetKeyDown(KeyCode.E))
        {
            Activate();
        }

        if (activated)
            MoveBox();
    }

    public void Activate()
    {
        if (!hasBeenMovedOnce)
            hasBeenMovedOnce = true;

        if (movementFinished) return;
        activated = true;
    }

    private void MoveBox()
    {
        Vector3 targetPos;

        if (mode == BoxMode.SingleOffset)
        {
            targetPos = singleTargetPos;
        }
        else
        {
            if (goalSpots == null || goalSpots.Length == 0)
            {
                movementFinished = true;
                activated = false;
                return;
            }
            targetPos = goalSpots[currentTargetIndex].position;
        }

        targetPos.y = startPos.y;

        transform.position = Vector3.MoveTowards(
            transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            activated = false;

            if (mode == BoxMode.SingleOffset)
            {
                movementFinished = true;
                Debug.Log("Box moved to target!");
            }
            else
            {
                currentTargetIndex++;
                if (currentTargetIndex >= goalSpots.Length)
                {
                    movementFinished = true;
                    Debug.Log("Puzzle box reached final goal!");

                    if (keyObject != null)
                        keyObject.SetActive(true);
                }
            }
        }
    }
}