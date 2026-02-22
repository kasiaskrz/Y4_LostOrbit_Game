using UnityEngine;

public class Doors : MonoBehaviour, IInteractable
{
    public Transform doorMesh;
    public Vector3 openOffset = new Vector3(0f, 0f, 2f);
    public float speed = 6f;

    Vector3 closedPos;
    bool isOpen;

    void Awake()
    {
        if (doorMesh == null)
            doorMesh = transform;

        closedPos = doorMesh.localPosition;
    }

    void Update()
    {
        Vector3 target = isOpen
            ? closedPos + openOffset
            : closedPos;

        doorMesh.localPosition =
            Vector3.MoveTowards(
                doorMesh.localPosition,
                target,
                speed * Time.deltaTime
            );
    }

    public void Interact()
    {
        isOpen = true;
    }
}
