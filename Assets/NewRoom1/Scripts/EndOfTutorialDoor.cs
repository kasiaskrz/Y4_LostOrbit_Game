using System.Collections;
using UnityEngine;

/// <summary>
/// Add this alongside TeleportOnTrigger on the MainHall SC005 door.
/// Disables TeleportOnTrigger briefly, shows "End of Tutorial", then fires the transition.
/// </summary>
public class EndOfTutorialDoor : MonoBehaviour
{
    [Tooltip("Assign the RoomUIManager in MainHall.")]
    public RoomUIManager roomUIManager;

    [Tooltip("How long to show the message before transitioning.")]
    public float messageDelay = 3f;

    [Header("Scene Transition")]
    public string sceneToLoad = "Room01";
    public string targetSpawnID = "TeleportDestination";

    private bool hasTriggered = false;
    private TeleportOnTrigger teleporter;

    private void Start()
    {
        teleporter = GetComponent<TeleportOnTrigger>();

        // Disable the teleporter so it doesn't fire instantly
        if (teleporter != null)
            teleporter.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;

        hasTriggered = true;
        StartCoroutine(ShowMessageThenTransition());
    }

    private IEnumerator ShowMessageThenTransition()
    {
        if (roomUIManager != null)
            roomUIManager.ShowMessage("End of Tutorial", messageDelay);

        yield return new WaitForSeconds(messageDelay);

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.TransitionToScene(sceneToLoad, targetSpawnID);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}