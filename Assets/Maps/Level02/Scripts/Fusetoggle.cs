using UnityEngine;

public class FuseToggle : MonoBehaviour
{
    [Header("Fuse Visual")]
    public GameObject fuseObject;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip takeSound1;
    public AudioClip takeSound2;
    public AudioClip placeSound;
    [Range(0f, 10f)] public float takeVolume = 1f;
    [Range(0f, 10f)] public float placeVolume = 1f;

    public void ToggleFuse()
    {
        if (fuseObject == null) return;

        bool isCurrentlyActive = fuseObject.activeSelf;
        fuseObject.SetActive(!isCurrentlyActive);

        if (isCurrentlyActive)
            PlayTakeSounds();
        else
            PlayPlaceSound();
    }

    public void ForceTake()
    {
        if (fuseObject == null) return;

        fuseObject.SetActive(false);
        PlayTakeSounds();
    }

    public void ForcePlace()
    {
        if (fuseObject == null) return;

        fuseObject.SetActive(true);
        PlayPlaceSound();
    }

    public void SetVisualState(bool isPlaced)
    {
        if (fuseObject == null) return;

        fuseObject.SetActive(isPlaced);
    }

    private void PlayTakeSounds()
    {
        if (audioSource == null) return;

        if (takeSound1 != null)
            audioSource.PlayOneShot(takeSound1, takeVolume);

        if (takeSound2 != null)
            audioSource.PlayOneShot(takeSound2, takeVolume);
    }

    private void PlayPlaceSound()
    {
        if (audioSource == null || placeSound == null) return;

        audioSource.PlayOneShot(placeSound, placeVolume);
    }
}