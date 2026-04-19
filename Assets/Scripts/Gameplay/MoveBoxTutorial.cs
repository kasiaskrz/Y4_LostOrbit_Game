using UnityEngine;

public class MoveBoxTutorial : MonoBehaviour
{
    [Header("Settings")]
    public float pushDistance = 1.5f;
    public float moveSpeed = 2f;
    public float interactRange = 2f;

    private Transform player;
    private Vector3 targetPos;
    private bool activated = false;
    public bool hasBeenMoved = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        targetPos = transform.position + transform.forward * pushDistance;
        targetPos.y = transform.position.y;
    }

    void Update()
    {
        if (hasBeenMoved || player == null) return;
        if (!activated &&
            Vector3.Distance(player.position, transform.position) < interactRange &&
            Input.GetKeyDown(KeyCode.E))
        {
            activated = true;
        }
        if (activated)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            if (Physics.Raycast(transform.position, dir, 0.6f))
            {
                hasBeenMoved = true;
                activated = false;
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                hasBeenMoved = true;
                activated = false;
            }
        }
    }
}