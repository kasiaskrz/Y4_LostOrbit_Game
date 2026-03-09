using UnityEngine;

public class LaserGate : MonoBehaviour
{
    public GameObject laserObject;

    public void DisableLaser()
    {
        if (laserObject != null)
        {
            laserObject.SetActive(false);
        }
    }
}