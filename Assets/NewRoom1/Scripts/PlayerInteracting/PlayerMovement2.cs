using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement2 : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public bool canMove = true; // tutorial can toggle this

    [Header("Gravity")]
    public float gravity = -9.81f;

    private CharacterController controller;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public bool HasTriedToMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
    }

    void Update()
    {
        // Always apply gravity so CharacterController stays grounded nicely
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // small downward force to keep grounded
        verticalVelocity += gravity * Time.deltaTime;

        if (!canMove)
        {
            controller.Move(Vector3.up * (verticalVelocity * Time.deltaTime));
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * h + transform.forward * v);
        if (move.sqrMagnitude > 1f) move.Normalize();

        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}
