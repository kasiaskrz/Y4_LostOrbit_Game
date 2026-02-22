using UnityEngine;

public class MoveBoxTutorial : MonoBehaviour
{
    public Vector3 targetOffset = new Vector3(-2f, 0f, -2f);
    public float moveSpeed = 2f;
    public float interactRange = 2f;

    private Transform player;
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool activated = false;
    private bool hasBeenMoved = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPos = transform.position;

        targetOffset.y = 0;
        targetPos = startPos + targetOffset;
    }

    void Update()
    {
        if (!activated && !hasBeenMoved && Vector3.Distance(player.position, transform.position) < interactRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                activated = true;
            }
        }

        if (activated)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                activated = false;
                hasBeenMoved = true;

                // ⭐ Notify the tutorial manager
                var tutorial = Object.FindFirstObjectByType<TutorialManager>();
                if (tutorial != null)
                    tutorial.NotifyCrateMoved();
            }
        }
    }
}
