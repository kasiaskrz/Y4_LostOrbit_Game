using UnityEngine;

public class MovableBox : MonoBehaviour
{
    public Vector3 targetOffset = new Vector3(-2f, 0f, -2f);
    public float moveSpeed = 2f;
    public float interactRange = 2f;

    [HideInInspector] public bool isBeingMoved = false;
    [HideInInspector] public bool hasBeenMoved = false;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool activated = false;

    private Transform player;

    void Start()
    {
        startPos = transform.position;

        targetOffset.y = 0;
        targetPos = startPos + targetOffset;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!activated && !hasBeenMoved && Vector3.Distance(player.position, transform.position) < interactRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                activated = true;
                isBeingMoved = true;
            }
        }

        if (activated)
        {
            Vector3 current = transform.position;
            current.y = startPos.y;

            Vector3 nextTarget = targetPos;
            nextTarget.y = startPos.y;

            transform.position = Vector3.MoveTowards(current, nextTarget, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, nextTarget) < 0.05f)
            {
                activated = false;
                isBeingMoved = false;
                hasBeenMoved = true; // <-- important: permanent state

                // ⭐ TELL THE TUTORIAL WE MOVED THE CRATE ⭐
                var tutorial = Object.FindFirstObjectByType<TutorialManager>();
                if (tutorial != null)
                    tutorial.NotifyCrateMoved();
            }
        }
    }

    public void Activate()
    {
        if (!hasBeenMoved)
        {
            activated = true;
            isBeingMoved = true;
        }
    }
}
