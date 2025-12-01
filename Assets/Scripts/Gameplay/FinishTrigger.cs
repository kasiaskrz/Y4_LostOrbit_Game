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
        Debug.Log("🚪 FINISHTRIGGER STARTED on: " + gameObject.name);

        col = GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString(lockedHex, out lockedColor);
        ColorUtility.TryParseHtmlString(unlockedHex, out unlockedColor);

        //  Auto-unlock if key was already collected
        if (GameProgress.Instance != null && GameProgress.Instance.keyCollected)
        {
            Debug.Log("🚪 DOOR AUTO-UNLOCKED (key already collected).");
            EnableFinishZone();
        }
        else
        {
            // Door starts locked
            col.isTrigger = false;

            if (rend != null)
                rend.material.color = lockedColor;

            Debug.Log("🚪 DOOR STARTS LOCKED.");
        }
    }

    public void EnableFinishZone()
    {
        Debug.Log("🚪 EnableFinishZone() CALLED");

        col.isTrigger = true;

        if (rend != null)
            rend.material.color = unlockedColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("🚪 Door Trigger Enter: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log(" Player entering door → loading SC005...");
            SceneManager.LoadScene("SC005");
        }
    }
}
