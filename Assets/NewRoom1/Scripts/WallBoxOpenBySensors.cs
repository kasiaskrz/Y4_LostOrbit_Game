using UnityEngine;

public class WallBoxOpenBySensors : MonoBehaviour
{
    public BeamSensor sensorA;
    public BeamSensor sensorB;

    [Header("Box part to move")]
    public Transform boxSide;              // drag the movable side/lid here

    [Header("Open movement")]
    public Vector3 openOffset = new Vector3(0f, 0f, -0.3f); // tweak direction
    public float openSpeed = 2f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool opened = false;

    void Start()
    {
        if (boxSide == null) return;

        closedPos = boxSide.localPosition;
        openPos = closedPos + openOffset;
    }

    void Update()
    {
        if (opened || boxSide == null) return;
        if (sensorA == null || sensorB == null) return;

        if (sensorA.isActive && sensorB.isActive)
        {
            opened = true;
        }

        if (opened)
        {
            boxSide.localPosition = Vector3.Lerp(
                boxSide.localPosition,
                openPos,
                Time.deltaTime * openSpeed
            );
        }
    }
}
