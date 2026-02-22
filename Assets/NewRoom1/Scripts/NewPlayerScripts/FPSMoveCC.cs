using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSMoveCC : MonoBehaviour
{
    public float speed = 4.5f;
    public float gravity = -20f;

    CharacterController cc;
    Vector3 velocity;

    void Awake() => cc = GetComponent<CharacterController>();

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z).normalized;
        cc.Move(move * speed * Time.deltaTime);

        if (cc.isGrounded && velocity.y < 0f) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
    }
}
