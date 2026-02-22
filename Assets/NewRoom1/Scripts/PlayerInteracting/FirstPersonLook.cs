using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    public Transform cam;
    public float sensitivity = 3f;   // raise this if you want faster turning

    float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // keep whatever initial camera pitch you set in editor (no snap)
        pitch = cam.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Yaw (turn left/right) - NO deltaTime
        transform.Rotate(0f, mouseX * sensitivity, 0f);

        // Pitch (look up/down) - NO deltaTime
        pitch -= mouseY * sensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        cam.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
