using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private BoxCollider col;
    private Renderer rend;

    [Header("Hex Colors")]
    public string lockedHex = "#535353"; 
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

        // Initial state
        col.isTrigger = false;

        if (rend != null)
            rend.material.color = lockedColor;
    }

    public void EnableFinishZone()
    {
        col.isTrigger = true;

        if (rend != null)
            rend.material.color = unlockedColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.instance.FinishLevel();
        }
    }
}
