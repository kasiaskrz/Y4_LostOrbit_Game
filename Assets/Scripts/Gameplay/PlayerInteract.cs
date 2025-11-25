using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider hit in hits)
        {
            MovableBox box = hit.GetComponent<MovableBox>();
            if (box != null)
            {
                box.Activate();
                Debug.Log("Box activated!");
                return;
            }
        }
    }
}
