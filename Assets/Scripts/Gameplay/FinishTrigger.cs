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

    [Header("Scene to load")]
    public string sceneToLoad = "MainHall";

    void Start()
    {
        Debug.Log(" FINISHTRIGGER STARTED on: " + gameObject.name);

        col = GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString(lockedHex, out lockedColor);
        ColorUtility.TryParseHtmlString(unlockedHex, out unlockedColor);

        // Auto-unlock if key was already collected
        if (GameProgress.Instance != null && GameProgress.Instance.keyCollected)
        {
            Debug.Log(" DOOR AUTO-UNLOCKED (key already collected).");
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
        if (!other.CompareTag("Player")) return;

        //  Prevent crash if GameProgress.Instance isn't ready yet
        if (GameProgress.Instance == null) return;

        //  Only teleport when key is collected
        if (!GameProgress.Instance.keyCollected) return;

        SceneManager.LoadScene(sceneToLoad);
    }

}
