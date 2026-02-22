using UnityEngine;

public class ShotTracer : MonoBehaviour
{
    public LineRenderer lr;
    public float life = 0.08f;

    float t;
    float startWidth;
    float startEndWidth;

    void Awake()
    {
        if (!lr) lr = GetComponent<LineRenderer>();
        startWidth = lr.startWidth;
        startEndWidth = lr.endWidth;
    }

    public void Init(Vector3 start, Vector3 end)
    {
        if (!lr) lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        // reset in case prefab is reused/changed
        lr.startWidth = startWidth;
        lr.endWidth = startEndWidth;
    }

    void Update()
    {
        t += Time.deltaTime;
        float a = 1f - (t / Mathf.Max(0.0001f, life)); // 1 -> 0

        lr.startWidth = startWidth * a;
        lr.endWidth = startEndWidth * a;

        if (t >= life)
            Destroy(gameObject);
    }
}
