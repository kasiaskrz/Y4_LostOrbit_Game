using UnityEngine;
using System.Collections;

public class PushBox : MonoBehaviour, IInteractable
{
    public Transform targetPosition;
    public float moveSpeed = 2f;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;
    private bool isMoving = false;

    public string PromptText => "Push box";
    public void Interact() { }

    void Update()
    {
        if (playerInRange && !isMoving && Input.GetKeyDown(interactKey))
            StartCoroutine(MoveBox());
    }

    IEnumerator MoveBox()
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition.position) > 0.05f)
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

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) playerInRange = true; }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) playerInRange = false; }
}