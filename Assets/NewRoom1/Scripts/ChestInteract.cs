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

        opened = true;

        DisableBlockingCollider();

        // 🔥 REMOVE KEY HERE
        InventoryManager.Instance.TryRemoveItem(keyItemData, 1);

        Debug.Log("Chest opened!");
    }

    // In ChestInteract.cs
    void DisableBlockingCollider()
    {
        if (blockingCollider != null)
            blockingCollider.enabled = false;

        // Enable the power cell so the player can now interact with it
        if (powerCellCollider != null)
            powerCellCollider.enabled = true;
    }
}