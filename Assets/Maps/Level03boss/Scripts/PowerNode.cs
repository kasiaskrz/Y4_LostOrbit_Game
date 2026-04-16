using UnityEngine;

public class PowerNode : MonoBehaviour, IDamageableBoss
{
    [Header("Cell Bone")]
    public Transform cellBone;

    [Header("Positions")]
    public float loweredY = -0.02604f;
    public float raisedY = 0.0154f;

    [Header("Movement")]
    public float moveSpeed = 2f;

    private BossCore boss;

    private bool isActive = false;
    private bool isCompleted = false;
    private bool moveUp = false;
    private bool moveDown = false;

    private void Start()
    {
        SetInactiveState();
    }

    private void Update()
    {
        if (cellBone == null)
            return;

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

    public void ActivateNode(BossCore owningBoss)
    {
        boss = owningBoss;
        isActive = true;
        isCompleted = false;

        moveUp = true;
        moveDown = false;

        gameObject.SetActive(true);
    }

    public void TakeDamage(int damage)
    {
        if (!isActive)
            return;

        if (isCompleted)
            return;

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

        gameObject.SetActive(false);
    }

    private void CompleteNode()
    {
        isActive = false;
        isCompleted = true;

        moveUp = false;
        moveDown = true;

        if (boss != null)
        {
            boss.NotifyNodeDestroyed(this);
        }

        Debug.Log(name + " completed");
    }
}