using UnityEngine;

public class LaserGate : MonoBehaviour
{
    public GameObject[] laserObjects;

    [Header("Guidance")]
    public GameObject gatePopup; // assign "Disable the gate to proceed" panel

    public void DisableLaser()
    {
        if (laserObjects == null || laserObjects.Length == 0) return;
        for (int i = 0; i < laserObjects.Length; i++)
        {
            if (laserObjects[i] != null)
            {
                laserObjects[i].SetActive(false);
            }
        }

        // Hide the popup once gate is disabled
        if (gatePopup != null)
            gatePopup.SetActive(false);
    }
}