using UnityEngine;

public class MovableBox : MonoBehaviour
{
    public enum BoxMode
    {
        SingleOffset,   // tutorial / simple box
        MultiTarget     // puzzle box with multiple goal spots
    }

    [Header("Mode")]
    public BoxMode mode = BoxMode.SingleOffset;

    [Header("Shared Settings")]
    public float moveSpeed = 2f;
    public float interactRange = 2f;

    [Header("Single Offset Mode")]
    public Vector3 targetOffset = new Vector3(-2f, 0f, -2f);

    [Header("Multi Target Mode")]
    public Transform[] goalSpots;      // assign 3 spots for SC002 puzzle
    public GameObject keyObject;       // key to enable at final goal (optional)

    [HideInInspector] public bool hasBeenMovedOnce = false;   // for tutorial + text
    [HideInInspector] public bool movementFinished = false;   // true after final position

    private Transform player;
    private Vector3 startPos;
    private Vector3 singleTargetPos;
    private int currentTargetIndex = 0;
    private bool activated = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position;

        // pre-compute single target
        targetOffset.y = 0;
        singleTargetPos = startPos + targetOffset;

        if (keyObject != null)
            keyObject.SetActive(false); // key hidden at start
    }

    void Update()
    {
        if (movementFinished || player == null)
            return;

        // Press E near box to activate
        if (!activated &&
            Vector3.Distance(player.position, transform.position) < interactRange &&
            Input.GetKeyDown(KeyCode.E))
        {
            Activate();
        }

        if (activated)
        {
            MoveBox();
        }
    }

    public void Activate()
    {
        // first time ever interacting
        if (!hasBeenMovedOnce)
            hasBeenMovedOnce = true;

        if (movementFinished)
            return;

        activated = true;
    }

    private void MoveBox()
    {
        Vector3 targetPos;

        if (mode == BoxMode.SingleOffset)
        {
            targetPos = singleTargetPos;
        }
        else // MultiTarget
        {
            if (goalSpots == null || goalSpots.Length == 0)
            {
                movementFinished = true;
                activated = false;
                return;
            }

            Transform t = goalSpots[currentTargetIndex];
            targetPos = t.position;
        }

        // keep original Y
        targetPos.y = startPos.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // reached current target
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            activated = false;

            if (mode == BoxMode.SingleOffset)
            {
                movementFinished = true;
                NotifyTutorialCrateMoved();
            }
            else // MultiTarget
            {
                currentTargetIndex++;

                // final goal reached
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

    private void NotifyTutorialCrateMoved()
    {
        // only used in the tutorial room – safe no-op elsewhere
        var tutorial = UnityEngine.Object.FindFirstObjectByType<TutorialManager>();
        if (tutorial != null)
            tutorial.NotifyCrateMoved();
    }
}
