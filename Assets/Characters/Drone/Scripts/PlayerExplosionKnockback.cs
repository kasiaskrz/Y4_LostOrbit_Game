using UnityEngine;

public class PlayerExplosionKnockback : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;

    [Header("Knockback")]
    public float gravity = 28f;
    public float horizontalDamp = 2.5f;
    public float verticalDamp = 1.8f;

    private Vector3 knockbackVelocity;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (controller == null)
            return;

        if (knockbackVelocity.sqrMagnitude > 0.01f)
        {
            knockbackVelocity.y -= gravity * Time.deltaTime;

            controller.Move(knockbackVelocity * Time.deltaTime);

            knockbackVelocity.x = Mathf.Lerp(knockbackVelocity.x, 0f, horizontalDamp * Time.deltaTime);
            knockbackVelocity.z = Mathf.Lerp(knockbackVelocity.z, 0f, horizontalDamp * Time.deltaTime);
            knockbackVelocity.y = Mathf.Lerp(knockbackVelocity.y, 0f, verticalDamp * Time.deltaTime);
        }
    }

    public void AddKnockback(Vector3 force)
    {
        knockbackVelocity += force;
    }
}