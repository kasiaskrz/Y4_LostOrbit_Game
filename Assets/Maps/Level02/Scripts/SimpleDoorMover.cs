using UnityEngine;
using System.Collections;

public class SimpleDoorMover : MonoBehaviour
{
    public Transform targetPosition;
    public float moveSpeed = 3f;

    private bool isMoving = false;

    public void MoveDoor()
    {
        if (isMoving || targetPosition == null) return;
        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition.position;
        isMoving = false;
    }
}