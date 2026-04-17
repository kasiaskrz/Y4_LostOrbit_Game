using UnityEngine;

public class RobotAudio : MonoBehaviour
{
    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip[] metalFootsteps;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    [Header("Weapon")]
    public AudioSource weaponSource;
    public AudioClip fireClip;

    // 🔊 FOOTSTEP (called from animation event)
    public void PlayFootstep()
    {
        if (footstepSource == null || metalFootsteps.Length == 0) return;

        AudioClip clip = metalFootsteps[Random.Range(0, metalFootsteps.Length)];

        footstepSource.pitch = Random.Range(minPitch, maxPitch);
        footstepSource.PlayOneShot(clip);
    }

    // 🔫 SHOOT (can be called from weapon or animation)
    public void PlayFire()
    {
        if (weaponSource == null || fireClip == null) return;

        weaponSource.pitch = Random.Range(0.95f, 1.05f);
        weaponSource.PlayOneShot(fireClip);
    }
}