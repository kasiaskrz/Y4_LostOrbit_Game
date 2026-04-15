using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    public Transform center;   // what it orbits around
    public float orbitSpeed = 3f;
    public float rotationSpeed = 10f;

    void Update()
    {
        // Orbit around center
        transform.RotateAround(center.position, Vector3.up, orbitSpeed * Time.deltaTime);

        // Rotate planet itself
        transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
    }
}