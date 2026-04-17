using UnityEngine;
using UnityEngine.AI;

public class RobotFootsteps : MonoBehaviour
{
    [Header("Refs")]
    public AudioSource audioSource;
    public NavMeshAgent agent; // optional but recommended

    [Header("Footsteps")]
    public AudioClip[] stepClips;
    public float stepDistance = 2f; // distance per step
    public float minSpeed = 0.1f;

    [Header("Audio")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.05f;

    private Vector3 lastPosition;
    private float distanceMoved;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        float speed = GetSpeed();

        if (speed < minSpeed)
        {
            distanceMoved = 0f;
            lastPosition = transform.position;
            return;
        }

        float frameDistance = Vector3.Distance(transform.position, lastPosition);
        distanceMoved += frameDistance;

        if (distanceMoved >= stepDistance)
        {
            PlayStep();
            distanceMoved = 0f;
        }

        lastPosition = transform.position;
    }

    float GetSpeed()
    {
        if (agent != null)
            return agent.velocity.magnitude;

        return (transform.position - lastPosition).magnitude / Time.deltaTime;
    }

    void PlayStep()
    {
        if (audioSource == null || stepClips.Length == 0) return;

        AudioClip clip = stepClips[Random.Range(0, stepClips.Length)];

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip);
    }
}