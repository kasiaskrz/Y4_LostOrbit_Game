using UnityEngine;

public class FuseToggle : MonoBehaviour
{
    [Header("Interaction")]
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public LayerMask interactLayer = ~0;

    [Header("Fuse Visual")]
    public GameObject fuseObject;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip takeSound1;
    public AudioClip takeSound2;
    public AudioClip placeSound;
    [Range(0f, 10f)] public float takeVolume = 1f;
    [Range(0f, 10f)] public float placeVolume = 1f;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(OptionsManager.Interact))
            TryInteract();
    }

    private void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                ToggleFuse();
    }

    private void ToggleFuse()
    {
        if (fuseObject == null) return;
        bool isCurrentlyActive = fuseObject.activeSelf;
        fuseObject.SetActive(!isCurrentlyActive);
        if (isCurrentlyActive) PlayTakeSounds();
        else PlayPlaceSound();
    }

    void PlayTakeSounds()
    {
        if (audioSource == null) return;
        if (takeSound1 != null) audioSource.PlayOneShot(takeSound1, takeVolume);
        if (takeSound2 != null) audioSource.PlayOneShot(takeSound2, takeVolume);
    }

    void PlayPlaceSound()
    {
        if (audioSource == null || placeSound == null) return;
        audioSource.PlayOneShot(placeSound, placeVolume);
    }
}