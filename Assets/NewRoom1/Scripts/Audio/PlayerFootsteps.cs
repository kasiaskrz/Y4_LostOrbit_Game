using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footstepClips;

    [Header("Timing")]
    public float walkStepDelay = 0.5f;
    public float sprintStepDelay = 0.3f;

    [Header("Movement")]
    public CharacterController controller;
    public float sprintSpeedThreshold = 6f;

    float stepTimer;

    void Update()
    {
        if (NotePickup.IsOpen) return;

        if (controller == null) return;

        float speed = controller.velocity.magnitude;

        if (controller.isGrounded && speed > 0.1f)
        {
            float delay = speed > sprintSpeedThreshold ? sprintStepDelay : walkStepDelay;

            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayFootstep();
                stepTimer = delay;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip);
    }
}