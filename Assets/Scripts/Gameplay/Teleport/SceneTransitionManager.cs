using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;
    }

    public void TransitionToScene(string sceneName, string targetSpawnID)
    {
        StartCoroutine(DoTransition(sceneName, targetSpawnID));
    }

    private IEnumerator DoTransition(string sceneName, string targetSpawnID)
    {
        // Fade to black
        yield return StartCoroutine(Fade(1f));

        // Load new scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Find player in new scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Find matching destination
            TeleportDestination[] dests = FindObjectsOfType<TeleportDestination>();
            foreach (var d in dests)
            {
                if (d.destinationID == targetSpawnID)
                {
                    // If using CharacterController, briefly disable it
                    var cc = player.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;

                    player.transform.SetPositionAndRotation(d.transform.position, d.transform.rotation);

                    if (cc != null) cc.enabled = true;
                    break;
                }
            }
        }

        // Fade back in
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null)
            yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}
