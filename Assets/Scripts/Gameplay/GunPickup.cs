using UnityEngine;

public class GunPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerGun gun = other.GetComponent<PlayerGun>();
            if (gun != null)
            {
                gun.GiveGun();
            }

            Destroy(gameObject);
        }
    }
}
