using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    public bool canMove = false; // tutorial will toggle this

    public bool HasTriedToMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
    }

    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;

        controller.Move(move * speed * Time.deltaTime);

        if (!canMove)
        {
            // ignore movement input
            return;
        }
    }


}
