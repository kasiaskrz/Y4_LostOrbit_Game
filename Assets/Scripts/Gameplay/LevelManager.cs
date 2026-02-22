using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private bool keyCollected = false;
    private float timer = 0f;
    private bool levelFinished = false;

    public Door door; // assign your door here

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (!levelFinished)
        {
            timer += Time.deltaTime;
        }
    }

    public void KeyCollected()
    {
        keyCollected = true;
        Debug.Log("Key collected!");

        if (door != null)
            door.OpenDoor();

        // Enable finish zone because key was collected
        FindFirstObjectByType<FinishTrigger>().EnableFinishZone();
    }

    public void FinishLevel()
    {
        if (!keyCollected)
        {
            Debug.Log("You need the key before finishing!");
            return;
        }

        levelFinished = true;
        Debug.Log("Level Finished! Time: " + timer.ToString("F2"));
    }

}
