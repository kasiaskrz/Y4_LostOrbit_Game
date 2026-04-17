using UnityEngine;

public class Barricade : MonoBehaviour
{
    public float targetY = 0.8f;
    public float moveSpeed = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip movingLoopSound;

    private bool isRising = false;
    private bool hasStartedAudio = false;

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = new Vector3(
            transform.position.x,
            targetY,
            transform.position.z
        );
    }

    private void Update()
    {
        if (!isRising) return;

        // Start looping sound ONCE
        if (!hasStartedAudio && audioSource != null && movingLoopSound != null)
        {
            audioSource.clip = movingLoopSound;
            audioSource.loop = true;
            audioSource.Play();
            hasStartedAudio = true;
        }

        // Move barricade
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Stop when finished
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isRising = false;

            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public void Activate()
    {
        isRising = true;
    }
}