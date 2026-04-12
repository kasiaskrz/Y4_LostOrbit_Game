using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [Header("Bridge Movement")]
    public float activatedYPosition = -0.27f;
    public float moveSpeed = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip movingSound;
    public AudioClip releaseSound;

    [Range(0f, 10f)] public float movingVolume = 5f;
    [Range(0f, 10f)] public float releaseVolume = 5f;

    private bool isActivated = false;
    private bool hasFinished = false;

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = new Vector3(
            transform.position.x,
            activatedYPosition,
            transform.position.z
        );

        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (!isActivated || hasFinished) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (!audioSource.isPlaying)
        {
            PlayMovingSound();
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            FinishMovement();
        }
    }

    public void ActivateBridge()
    {
        if (isActivated) return;

        isActivated = true;
        Debug.Log("Bridge lowering.");
    }

    void PlayMovingSound()
    {
        if (audioSource != null && movingSound != null)
        {
            audioSource.clip = movingSound;
            audioSource.loop = true;
            audioSource.volume = movingVolume;
            audioSource.Play();
        }
    }

    void FinishMovement()
    {
        hasFinished = true;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (audioSource != null && releaseSound != null)
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(releaseSound, releaseVolume);
        }
    }
}