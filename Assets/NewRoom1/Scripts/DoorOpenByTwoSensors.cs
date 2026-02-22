using UnityEngine;

public class DoorOpenByTwoSensors : MonoBehaviour
{
    public BeamSensor sensorA;
    public BeamSensor sensorB;

    [Header("What moves")]
    public Transform doorTransform;

    [Header("Open settings")]
    public Vector3 openLocalOffset = new Vector3(0f, 0.25f, 0f); // move up
    public float openSpeed = 2f;

    Vector3 closedPos;
    Vector3 openPos;
    bool opened;

    void Start()
    {
        if (doorTransform == null) return;
        closedPos = doorTransform.localPosition;
        openPos = closedPos + openLocalOffset;
    }

    void Update()
    {
        if (doorTransform == null) return;

        if (!opened && sensorA != null && sensorB != null && sensorA.isActive && sensorB.isActive)
            opened = true;

        if (opened)
            doorTransform.localPosition = Vector3.MoveTowards(
                doorTransform.localPosition,
                openPos,
                openSpeed * Time.deltaTime
            );
    }
}
