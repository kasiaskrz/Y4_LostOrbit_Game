using UnityEngine;

public class PowerNode : MonoBehaviour, IDamageable
{
    [Header("Cell Bone")]
    public Transform cellBone;

    [Header("Positions")]
    public float loweredY = -0.02604f; // CLOSED
    public float raisedY = 0.0154f;    // OPEN

    [Header("Movement")]
    public float moveSpeed = 2f;

    private BossController boss;

    private bool isActive = false;
    private bool isCompleted = false;
    private bool moveUp = false;
    private bool moveDown = false;

    void Start()
    {
        // FORCE correct starting position
        if (cellBone != null)
        {
            Vector3 pos = cellBone.localPosition;
            pos.y = loweredY;
            cellBone.localPosition = pos;
        }

        isActive = false;
        isCompleted = false;
    }

    void Update()
    {
        if (cellBone == null) return;

        Vector3 pos = cellBone.localPosition;

        if (moveUp)
        {
            pos.y = Mathf.Lerp(pos.y, raisedY, Time.deltaTime * moveSpeed);
            cellBone.localPosition = pos;

            if (Mathf.Abs(pos.y - raisedY) < 0.0001f)
            {
                pos.y = raisedY;
                cellBone.localPosition = pos;
                moveUp = false;
            }
        }

        if (moveDown)
        {
            pos.y = Mathf.Lerp(pos.y, loweredY, Time.deltaTime * moveSpeed);
            cellBone.localPosition = pos;

            if (Mathf.Abs(pos.y - loweredY) < 0.0001f)
            {
                pos.y = loweredY;
                cellBone.localPosition = pos;
                moveDown = false;
            }
        }
    }

    public void ActivateNode(BossController bossController)
    {
        if (isCompleted) return;

        boss = bossController;
        isActive = true;

        moveUp = true;
        moveDown = false;
    }

    public void TakeDamage(float amount)
    {
        if (!isActive) return;
        if (isCompleted) return;

        CompleteNode();
    }
    public void SetInactiveState()
{
    isActive = false;
    isCompleted = false;

    moveUp = false;
    moveDown = false;

    if (cellBone != null)
    {
        Vector3 pos = cellBone.localPosition;
        pos.y = loweredY;
        cellBone.localPosition = pos;
    }
}
    void CompleteNode()
    {
        isActive = false;
        isCompleted = true;

        moveUp = false;
        moveDown = true;

        if (boss != null)
        {
            boss.DisableShield();
        }

        Debug.Log(name + " completed");
    }
}