using UnityEngine;

public class PlateAnimation : MonoBehaviour
{
    public Vector3 loweredOffset = new Vector3(0, -0.2f, 0);
    public float speed = 2f;

    private bool lower = false;
    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        startPos = transform.localPosition;
        targetPos = startPos + loweredOffset;
    }

    public void LowerPlate()
    {
        lower = true;
    }

    void Update()
    {
        if (lower)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPos,
                Time.deltaTime * speed
            );
        }
    }
}
