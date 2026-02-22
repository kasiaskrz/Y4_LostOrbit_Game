using UnityEngine;

public class WeaponViewmodel : MonoBehaviour
{
    [Header("Animator")]
    public Animator armsAnimator;

    [Header("Fire Settings")]
    public float fireCooldown = 0.12f;

    float nextFireTime;

    void Update()
    {
        if (armsAnimator == null) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;

            armsAnimator.ResetTrigger("Fire");
            armsAnimator.SetTrigger("Fire");
        }
    }
}
