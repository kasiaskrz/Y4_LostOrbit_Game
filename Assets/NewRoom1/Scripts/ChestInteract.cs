using UnityEngine;

public class ChestInteract : MonoBehaviour, IInteractable
{
    public Animation anim;
    public string clipName = "ChestAnim";

    private bool opened = false;

    void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animation>();

        // Force chest to start CLOSED
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

        // 🔑 check simple key flag
        if (!KeyPickup2.KeyCollected)
        {
            Debug.Log("Chest locked. Key required.");
            return;
        }

        if (anim == null || anim.GetClip(clipName) == null)
        {
            Debug.LogError("Chest animation missing!");
            return;
        }

        anim[clipName].speed = 1f;
        anim.Play(clipName);
        opened = true;

        Debug.Log("Chest opened!");
    }
}
