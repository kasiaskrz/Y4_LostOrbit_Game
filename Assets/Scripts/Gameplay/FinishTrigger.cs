using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private BoxCollider col;
    private Renderer rend;

    [Header("Hex Colors")]
    public string lockedHex = "#535353ff";
    public string unlockedHex = "#D9D9D9ff";

    private Color lockedColor;
    private Color unlockedColor;

    [Header("Scene to load")]
    public string sceneToLoad = "MainHall";

    [Tooltip("Spawn ID in the destination scene.")]
    public string targetSpawnID = "Door1";

    [Header("Unlock Condition")]
    [Tooltip("Which room this door belongs to. Determines which flag to check.")]
    public RoomID roomID = RoomID.SC002;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString(lockedHex, out lockedColor);
        ColorUtility.TryParseHtmlString(unlockedHex, out unlockedColor);

        // Auto unlock if already complete
        if (IsRoomComplete())
        {
            Debug.Log("Door auto-unlocked on start.");
            EnableFinishZone();
        }
        else
        {
            col.isTrigger = false;
            if (rend != null) rend.material.color = lockedColor;
            Debug.Log("Door starts locked.");
        }
    }

    public void EnableFinishZone()
    {
        Debug.Log("EnableFinishZone() called.");
        col.isTrigger = true;
        if (rend != null) rend.material.color = unlockedColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!IsRoomComplete()) return;

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.TransitionToScene(sceneToLoad, targetSpawnID);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }

    private bool IsRoomComplete()
    {
        if (GameProgress.Instance == null) return false;

        switch (roomID)
        {
            case RoomID.SC002: return GameProgress.Instance.sc002Complete;
            case RoomID.SC003: return GameProgress.Instance.sc003Complete;
            default: return GameProgress.Instance.keyCollected;
        }
    }
}

public enum RoomID
{
    SC002,
    SC003
}