using UnityEngine;

/// <summary>
/// Place on an empty GameObject in Room01.
/// Shows "Chapter 1" using the existing RoomUIManager when the scene loads.
/// </summary>
public class Chapter1Intro : MonoBehaviour
{
    [Tooltip("Assign the RoomUIManager already in Room01.")]
    public RoomUIManager roomUIManager;

    private void Start()
    {
        if (roomUIManager != null)
            roomUIManager.ShowMessage("Chapter 1", 4f);
    }
}