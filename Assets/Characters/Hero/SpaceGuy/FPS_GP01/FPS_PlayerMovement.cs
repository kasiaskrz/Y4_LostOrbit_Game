using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPS_PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cameraPivot;
    public Camera playerCamera;
    public PlayerExplosionKnockback explosionKnockback;

    [Header("Mouse Look")]
    public float sensitivity = 2.0f;
    public float yClamp = 85f;
    public float lookSmooth = 12f;

    [Header("Movement")]
    public float walkSpeed = 4.5f;
    public float sprintSpeed = 7.0f;
    public float accel = 12f;
    public float airControl = 3.5f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -20f;
    public float groundedStick = -2f;

    [Header("Feel (Optional)")]
    public bool enableHeadbob = true;
    public float bobAmount = 0.04f;
    public float bobSpeed = 12f;

    public bool enableFovKick = true;
    public float walkFov = 60f;
    public float sprintFov = 68f;
    public float fovLerp = 10f;

    CharacterController cc;

    float pitch;
    Vector2 currentLook;
    Vector2 lookVel;

    Vector3 velocity;
    Vector3 currentMove;

    Vector3 camStartLocalPos;
    float bobTimer;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        if (!cameraPivot)
            Debug.LogError("PlayerMovement: cameraPivot not assigned.");

        if (!playerCamera && cameraPivot)
            playerCamera = cameraPivot.GetComponentInChildren<Camera>();

        if (explosionKnockback == null)
            explosionKnockback = GetComponent<PlayerExplosionKnockback>();

        if (playerCamera)
            camStartLocalPos = playerCamera.transform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera && enableFovKick)
            playerCamera.fieldOfView = walkFov;
    }

    void Update()
    {
        if (Time.timeScale == 0f || NotePickup.IsOpen || LevelComplete.IsOpen) return;

        Look();
        Move();
        Headbob();
        FovKick();
    }

    void Look()
    {
        Vector2 targetLook = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * sensitivity;

        currentLook = Vector2.SmoothDamp(currentLook, targetLook, ref lookVel, 1f / lookSmooth);

        transform.Rotate(Vector3.up * currentLook.x);

        pitch -= currentLook.y;
        pitch = Mathf.Clamp(pitch, -yClamp, yClamp);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        bool grounded = cc.isGrounded;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(x, 0f, z);
        input = Vector3.ClampMagnitude(input, 1f);

        Vector3 desired = (transform.right * input.x + transform.forward * input.z) * speed;

        float currentAccel = grounded ? accel : airControl;
        currentMove = Vector3.Lerp(currentMove, desired, currentAccel * Time.deltaTime);

        if (grounded && velocity.y < 0f)
            velocity.y = groundedStick;

        if (grounded && Input.GetKeyDown(KeyCode.Space))
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;

        Vector3 knockback = Vector3.zero;

        if (explosionKnockback != null)
        {
            knockback = explosionKnockback.CurrentKnockback;

            if (grounded && knockback.y < 0f)
                knockback.y = 0f;

            explosionKnockback.TickDamping(Time.deltaTime);
        }

        Vector3 finalMove = currentMove + knockback + Vector3.up * velocity.y;
        cc.Move(finalMove * Time.deltaTime);
    }

    void Headbob()
    {
        if (!enableHeadbob || !playerCamera) return;

        Vector3 horiz = new Vector3(cc.velocity.x, 0f, cc.velocity.z);
        bool moving = horiz.magnitude > 0.2f && cc.isGrounded;

        if (!moving)
        {
            bobTimer = 0f;
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                camStartLocalPos,
                12f * Time.deltaTime
            );
            return;
        }

        bobTimer += Time.deltaTime * bobSpeed * (Input.GetKey(KeyCode.LeftShift) ? 1.25f : 1f);

        float bob = Mathf.Sin(bobTimer) * bobAmount;
        Vector3 target = camStartLocalPos + new Vector3(0f, bob, 0f);

        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            target,
            12f * Time.deltaTime
        );
    }

    void FovKick()
    {
        if (!enableFovKick || !playerCamera) return;

        float target = Input.GetKey(KeyCode.LeftShift) ? sprintFov : walkFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, target, fovLerp * Time.deltaTime);
    }
}