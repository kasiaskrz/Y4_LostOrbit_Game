using UnityEngine;

public enum AmmoVisualType
{
    None = 0,
    ShotgunShell = 1,
    RifleBullet = 2,
    PistolRound = 3,
    EnergyCell = 4
}

public interface IWeaponUIProvider
{
    int CurrentAmmo { get; }
    int MaxAmmo { get; }
    bool IsReloading { get; }
    AmmoVisualType AmmoType { get; }
}
