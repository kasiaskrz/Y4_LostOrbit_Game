using UnityEngine;
using TMPro;

public class WinSceneTimer : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    void Start()
    {
        float elapsed = PlayerPrefs.GetFloat("WinTime", 0f);

        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed % 60f);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}