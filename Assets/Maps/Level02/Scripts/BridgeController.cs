using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [Header("Bridge Movement")]
    public float activatedYPosition = -0.27f;
    public float moveSpeed = 2f;

    private bool isActivated = false;
    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = new Vector3(
            transform.position.x,
            activatedYPosition,
            transform.position.z
        );
    }

    private void Update()
    {
        if (!isActivated) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    public void ActivateBridge()
    {
        if (isActivated) return;

        isActivated = true;
        Debug.Log("Bridge lowering.");
    }
}