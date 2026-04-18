using UnityEngine;
public class AreaPopupTrigger : MonoBehaviour
{
    public GameObject popup;
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (popup != null) popup.SetActive(true);
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (popup != null) popup.SetActive(false);
    }
}