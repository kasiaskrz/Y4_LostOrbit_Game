using UnityEngine;
using System.Collections;

public class SimpleDoorMover : MonoBehaviour
{
    [Header("Movement")]
    public Transform targetPosition;
    public float moveSpeed = 3f;

    [Header("Screen Shake")]
    public float shakeDuration = 0.4f;
    public float shakeMagnitude = 0.22f;
    public bool shakeOnImpact = true;

    [Header("Audio")]
    public bool playAudioOnStart = true;   // plays when door starts moving
    public bool playAudioOnImpact = false; // plays when door reaches end

    private bool isMoving = false;
    private bool hasActivated = false;

    private CameraShake cameraShake;
    private AudioSource doorAudio;

    void Start()
    {
        doorAudio = GetComponent<AudioSource>();

        Camera cam = Camera.main;
        if (cam != null)
        {
            cameraShake = cam.GetComponent<CameraShake>();
        }
        else
        {
            Debug.LogWarning("SimpleDoorMover: No camera with MainCamera tag found in scene.");
        }
    }

    public void MoveDoor()
    {
        if (isMoving || hasActivated || targetPosition == null)
            return;

        hasActivated = true;

        if (playAudioOnStart && doorAudio != null)
        {
            doorAudio.pitch = Random.Range(0.95f, 1.05f);
            doorAudio.Play();
        }

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

        if (playAudioOnImpact && doorAudio != null)
        {
            doorAudio.pitch = Random.Range(0.95f, 1.05f);
            doorAudio.Play();
        }

        if (shakeOnImpact && cameraShake != null)
        {
            cameraShake.Shake(shakeDuration, shakeMagnitude);
        }

        isMoving = false;
    }
}