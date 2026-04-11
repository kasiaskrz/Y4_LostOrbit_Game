using UnityEngine;

public class PushBox : MonoBehaviour
{
    [Header("Rail Setup")]
    public Transform targetPosition;
    public float pushSpeed = 2f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Input")]
    public string verticalAxis = "Vertical"; // W/S or Up/Down

    private bool playerInRange = false;

    private Vector3 startPosition;
    private Vector3 railDirection;
    private float railLength;

    private AudioSource pushAudio;

    void Start()
    {
        startPosition = transform.position;

        if (targetPosition == null)
        {
            Debug.LogError($"PushBox on {gameObject.name} has no targetPosition assigned.");
            enabled = false;
            return;
        }

        Vector3 rail = targetPosition.position - startPosition;
        railLength = rail.magnitude;

        if (railLength <= 0.001f)
        {
            Debug.LogError($"PushBox on {gameObject.name} has targetPosition too close to start position.");
            enabled = false;
            return;
        }

        railDirection = rail.normalized;

        pushAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!playerInRange)
        {
            StopPushSound();
            return;
        }

        if (!Input.GetKey(interactKey))
        {
            StopPushSound();
            return;
        }

        float input = Input.GetAxisRaw(verticalAxis);

        if (Mathf.Abs(input) < 0.01f)
        {
            StopPushSound();
            return;
        }

        float moveAmount = input * pushSpeed * Time.deltaTime;

        Vector3 fromStart = transform.position - startPosition;
        float currentDistance = Vector3.Dot(fromStart, railDirection);
        float newDistance = Mathf.Clamp(currentDistance + moveAmount, 0f, railLength);

        bool isActuallyMoving = Mathf.Abs(newDistance - currentDistance) > 0.0001f;

        if (isActuallyMoving)
        {
            transform.position = startPosition + railDirection * newDistance;

            if (pushAudio != null && !pushAudio.isPlaying)
            {
                pushAudio.Play();
            }
        }
        else
        {
            StopPushSound();
        }
    }

    void StopPushSound()
    {
        if (pushAudio != null && pushAudio.isPlaying)
        {
            pushAudio.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StopPushSound();
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (targetPosition == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, targetPosition.position);
        Gizmos.DrawSphere(transform.position, 0.08f);
        Gizmos.DrawSphere(targetPosition.position, 0.08f);
    }
#endif
}