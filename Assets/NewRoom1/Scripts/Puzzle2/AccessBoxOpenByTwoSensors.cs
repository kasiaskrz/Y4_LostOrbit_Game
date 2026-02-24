using UnityEngine;

public class AccessBoxOpenByTwoSensors : MonoBehaviour, IInteractable
{
    [Header("Sensors")]
    public BeamSensor sensorA;
    public BeamSensor sensorB;

    [Header("Animator")]
    public Animator animator;
    public string openTrigger = "Open";

    private bool opened = false;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (opened) return;

        if (sensorA == null || sensorB == null) return;

        if (sensorA.isActive && sensorB.isActive)
        {
            OpenBox();
        }
        else
        {
            Debug.Log("Sensors not active.");
        }
    }

    void OpenBox()
    {
        opened = true;

        if (animator != null)
            animator.SetTrigger(openTrigger);
    }
}