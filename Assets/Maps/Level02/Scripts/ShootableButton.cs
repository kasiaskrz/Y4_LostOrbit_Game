using UnityEngine;

public class ShootableButton : MonoBehaviour, IDamageable
{
    [Header("Encounter")]
    public SniperEncounter encounter;

    [Header("Doors")]
    public SimpleDoorMover exitDoor;
    public SimpleDoorMover sniperDoor;

    [Header("Button Settings")]
    public float health = 1f;
    public bool destroyAfterShot = false;

    private bool activated = false;

    public void TakeDamage(float amount)
    {
        if (activated) return;

        health -= amount;

        if (health <= 0f)
        {
            ActivateButton();
        }
    }

    public void ActivateButton()
    {
        if (activated) return;
        activated = true;

        if (encounter != null)
        {
            encounter.DisableEncounter();
        }

        if (exitDoor != null)
        {
            exitDoor.MoveDoor();
        }

        if (sniperDoor != null)
        {
            sniperDoor.MoveDoor();
        }

        Debug.Log("Button activated. Sniper disabled and doors moved.");

        if (destroyAfterShot)
        {
            Destroy(gameObject);
        }
    }
}