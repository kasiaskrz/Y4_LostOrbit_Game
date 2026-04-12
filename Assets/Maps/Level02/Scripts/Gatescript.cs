using UnityEngine;

public class LaserGate : MonoBehaviour
{
    public GameObject[] laserObjects;

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
    }
}