using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    private BoxCollider col;
    private Renderer rend;

    [Header("Hex Colors")]
    public string lockedHex = "#000000ff";
    public string unlockedHex = "#ffffffff";

    private Color lockedColor;
    private Color unlockedColor;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString(lockedHex, out lockedColor);
        ColorUtility.TryParseHtmlString(unlockedHex, out unlockedColor);

        // Door starts locked
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
            // Teleport player to next scene
            SceneManager.LoadScene("SC005");
        }
    }
}
