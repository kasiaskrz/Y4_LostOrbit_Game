using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    private BoxCollider col;
    private Renderer rend;

    [Header("Hex Colors")]
    public string lockedHex = "#000000ff";
    public string unlockedHex = "#D9D9D9";

    private Color lockedColor;
    private Color unlockedColor;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();

        // Convert hex to Color
        ColorUtility.TryParseHtmlString(lockedHex, out lockedColor);
        ColorUtility.TryParseHtmlString(unlockedHex, out unlockedColor);

        // Default: locked
        col.isTrigger = false;
        if (rend != null)
            rend.material.color = lockedColor;

        // Auto-unlock this door if player already collected the key (from SC002)
        if (GameProgress.Instance.keyCollected)
        {
            EnableFinishZone();
            Debug.Log("Door auto-unlocked (player has key)");
        }
    }

    public void EnableFinishZone()
    {
        // Unlock door
        col.isTrigger = true;

        if (rend != null)
            rend.material.color = unlockedColor;

        Debug.Log("Door unlocked manually!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("SC005");
        }
    }
}
