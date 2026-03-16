using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    public enum MoveAxis
    {
        X,
        Y,
        Z
    }

    [Header("Movement")]
    public MoveAxis moveAxis = MoveAxis.Y;
    public float openDistance = 2f;
    public float moveSpeed = 2f;
    public bool closeWhenPlayerLeaves = true;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition;

        switch (moveAxis)
        {
            case MoveAxis.X:
                openPosition += new Vector3(openDistance, 0f, 0f);
                break;

            case MoveAxis.Y:
                openPosition += new Vector3(0f, openDistance, 0f);
                break;

            case MoveAxis.Z:
                openPosition += new Vector3(0f, 0f, openDistance);
                break;
        }
    }

    private void Update()
    {
        Vector3 target = isOpen ? openPosition : closedPosition;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    public void OpenDoor()
    {
        isOpen = true;
    }

    public void CloseDoor()
    {
        if (closeWhenPlayerLeaves)
            isOpen = false;
    }
}