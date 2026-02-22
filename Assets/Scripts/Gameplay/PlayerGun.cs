using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [Header("Setup")]
    public Camera cam;                
    public Transform playerBody;      
    public Transform gunModel;        
    public bool hasGun = false;

    [Header("Shooting")]
    public float shootDistance = 100f;
    public float damage = 100f;

    [Header("Aiming / Smoothness")]
    public float rotateSpeed = 10f;       
    public float gunFollowSpeed = 15f;    
    public float gunRadius = 0.6f;        
    public float gunHeightOffset = 1.0f;  

    [Header("Gun Rotation Offset")]
    public float gunYawOffset = 0f;   // <--- NEW OFFSET

    void Update()
    {
        if (!hasGun) return;

        AimTowardsMouse();

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void AimTowardsMouse()
    {
        if (cam == null || playerBody == null || gunModel == null) return;

        // Ray from camera through mouse
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.up, new Vector3(0f, playerBody.position.y, 0f));
        float enter;

        if (plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector3 dir = hitPoint - playerBody.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.0001f) return;

            Vector3 dirNorm = dir.normalized;

            // Smoothly rotate player
            Quaternion targetRot = Quaternion.LookRotation(dirNorm, Vector3.up);
            playerBody.rotation = Quaternion.Slerp(
                playerBody.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );

            // Smoothly move gun around the player
            Vector3 targetGunPos =
                playerBody.position +
                dirNorm * gunRadius +
                Vector3.up * gunHeightOffset;

            gunModel.position = Vector3.Lerp(
                gunModel.position,
                targetGunPos,
                gunFollowSpeed * Time.deltaTime
            );

            // Apply rotation offset to correct the gun’s facing
            Quaternion gunRot =
                Quaternion.LookRotation(dirNorm, Vector3.up) *
                Quaternion.Euler(0f, gunYawOffset, 0f);

            gunModel.rotation = Quaternion.Slerp(
                gunModel.rotation,
                gunRot,
                gunFollowSpeed * Time.deltaTime
            );
        }
    }

    void Shoot()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootDistance))
        {
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public void GiveGun()
    {
        hasGun = true;

        if (gunModel != null)
        {
            gunModel.gameObject.SetActive(true);
        }
    }
}
