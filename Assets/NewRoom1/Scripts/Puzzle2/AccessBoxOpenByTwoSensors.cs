using UnityEngine;

public class AccessBoxOpenByTwoSensors : MonoBehaviour
{
    public BeamSensor sensorA;
    public BeamSensor sensorB;

    public Animation anim;                 // legacy Animation component
    public bool forceClosedOnStart = true;

    bool opened = false;
    string clipToPlay;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animation>();

        if (anim != null)
        {
            // Prefer default clip
            clipToPlay = (anim.clip != null) ? anim.clip.name : null;

            // Otherwise grab first clip in the list
            if (string.IsNullOrEmpty(clipToPlay))
            {
                foreach (AnimationState st in anim)
                {
                    clipToPlay = st.name;
                    break;
                }
            }
        }
    }

    void Start()
    {
        // IMPORTANT: only do this in Play Mode, and after everything is initialized
        if (forceClosedOnStart) ForceClosedPose();
    }

    void ForceClosedPose()
    {
        if (anim == null || string.IsNullOrEmpty(clipToPlay)) return;

        anim.Play(clipToPlay);
        anim[clipToPlay].time = 0f;
        anim[clipToPlay].speed = 0f;
        anim.Sample();
        anim.Stop();
    }

    void Update()
    {
        if (opened) return;
        if (sensorA == null || sensorB == null || anim == null || string.IsNullOrEmpty(clipToPlay)) return;

        // Your BeamSensor MUST update this bool properly
        bool a = sensorA.isActive;
        bool b = sensorB.isActive;

        if (a && b)
        {
            opened = true;
            anim.Stop();
            anim.Play("Empty|EmptyAction");
        }

    }
}
