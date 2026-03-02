using UnityEngine;

public class ChestInteract : MonoBehaviour, IInteractable
{
    public Animation anim;
    public string clipName = "ChestAnim";

    private bool opened = false;

    public string PromptText => opened ? "" : !KeyPickup2.KeyCollected ? "Locked - Need Key" : "Open Chest";

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