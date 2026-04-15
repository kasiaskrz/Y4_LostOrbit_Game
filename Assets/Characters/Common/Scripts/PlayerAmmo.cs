using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    public int shotgunAmmo = 0;
    public int rifleAmmo = 0;

    public int maxShotgunAmmo = 50;
    public int maxRifleAmmo = 200;

    public void AddAmmo(AmmoVisualType type, int amount)
    {
        switch (type)
        {
            case AmmoVisualType.ShotgunShell:
                shotgunAmmo = Mathf.Clamp(shotgunAmmo + amount, 0, maxShotgunAmmo);
                break;

            case AmmoVisualType.RifleBullet:
                rifleAmmo = Mathf.Clamp(rifleAmmo + amount, 0, maxRifleAmmo);
                break;
        }
    }
}