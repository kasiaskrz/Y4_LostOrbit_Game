using UnityEngine;
using TMPro;

public class ProximityPrompt : MonoBehaviour
{
    [Header("Prompt Settings")]
    public string promptMessage = "Press E to interact";
    public float interactDistance = 2.5f;
    public Camera playerCamera;
    public LayerMask interactLayer = ~0;

    private static TextMeshProUGUI promptUI;
    private static int activePromptCount = 0;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Find the prompt UI text object in the scene if not cached
        if (promptUI == null)
        {
            GameObject promptObj = GameObject.Find("InteractionPromptText");
            if (promptObj != null)
                promptUI = promptObj.GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (playerCamera == null || promptUI == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                ShowPrompt();
                return;
            }
        }

        HidePrompt();
    }

    private void ShowPrompt()
    {
        if (promptUI != null)
        {
            promptUI.text = promptMessage;
            promptUI.gameObject.SetActive(true);
        }
    }

    private void HidePrompt()
    {
        if (promptUI != null)
        {
            promptUI.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        HidePrompt();
    }
}