using UnityEngine;

public class TeleportOnTrigger : MonoBehaviour
{
    [Tooltip("Name of the scene to load, as in Build Settings.")]
    public string sceneToLoad;

    [Tooltip("DestinationID of TeleportDestination in that scene.")]
    public string targetSpawnID;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TELEPORTER ACTIVE? " + this.enabled);

        if (!this.enabled)
        {
            Debug.Log("Teleport script is DISABLED, so no teleport should happen.");
            return;
        }
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.TransitionToScene(sceneToLoad, targetSpawnID);
            }
            else
            {
                Debug.LogWarning("SceneTransitionManager not found in scene.");
            }
        }
    }
}
