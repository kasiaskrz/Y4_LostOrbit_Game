using System.Collections;
using UnityEngine;

public class ChestInteract : MonoBehaviour, IInteractable
{
    public Animation anim;
    public string clipName = "ChestAnim";

    [Header("Blocking Collider (prevents accessing items inside)")]
    public Collider blockingCollider;

    [Header("Contained Item")]
    public Collider powerCellCollider;

    [Header("Required Key")]
    public ItemData keyItemData;

    [Header("Audio")]
    public AudioClip openSound;
    [Range(0f, 5f)]
    public float openVolume = 1f;

    private bool opened = false;

    public string PromptText =>
        opened ? "" :
        !InventoryManager.Instance.HasItem(keyItemData) ? "Locked - Need Key" : "Open Chest";

    void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animation>();

        if (anim != null && anim.GetClip(clipName) != null)
        {
            anim.Play(clipName);
            anim[clipName].time = 0f;
            anim[clipName].speed = 0f;
        }
    }

    public void Interact()
    {
        if (opened) return;

        if (!InventoryManager.Instance.HasItem(keyItemData))
        {
            Debug.Log("Chest locked. Key required.");
            return;
        }

        anim[clipName].speed = 1f;
        anim.Play(clipName);

        if (openSound != null)
            AudioSource.PlayClipAtPoint(openSound, transform.position, openVolume);

        opened = true;

        InventoryManager.Instance.TryRemoveItem(keyItemData, 1);

        StartCoroutine(EnableLootAfterAnimation());

        Debug.Log("Chest opened!");
    }

    IEnumerator EnableLootAfterAnimation()
    {
        if (anim == null || anim.GetClip(clipName) == null)
        {
            DisableBlockingCollider();
            yield break;
        }

        yield return new WaitForSeconds(anim[clipName].length);

        DisableBlockingCollider();
    }

    void DisableBlockingCollider()
    {
        if (blockingCollider != null)
            blockingCollider.enabled = false;

        if (powerCellCollider != null)
            powerCellCollider.enabled = true;
    }
}