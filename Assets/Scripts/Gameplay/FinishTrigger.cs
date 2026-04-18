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
    public string targetSpawnID = "Door2";

    [Header("Unlock Condition")]
    public RoomID roomID = RoomID.SC002;

    [Header("UI")]
    [Tooltip("Assign the room guide to show locked message.")]
    public SC002Guide sc002Guide;
    public SC003Guide sc003Guide;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString(lockedHex, out lockedColor);
        ColorUtility.TryParseHtmlString(unlockedHex, out unlockedColor);

        if (IsRoomComplete())
        {
            EnableFinishZone();
        }
        else
        {
            col.isTrigger = true; // Keep as trigger so we can detect attempts
            if (rend != null) rend.material.color = lockedColor;
        }
    }

    public void EnableFinishZone()
    {
        if (rend != null) rend.material.color = unlockedColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!IsRoomComplete())
        {
            // Show locked message
            if (roomID == RoomID.SC002 && sc002Guide != null)
                sc002Guide.ShowLockedExitMessage();
            else if (roomID == RoomID.SC003 && sc003Guide != null)
                sc003Guide.ShowLockedExitMessage();
            return;
        }

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