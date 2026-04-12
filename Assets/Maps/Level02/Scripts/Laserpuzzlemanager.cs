using UnityEngine;
using System.Collections;

public class LaserPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Order")]
    public LaserButton[] correctOrder;

    [Header("Gate")]
    public LaserGate linkedGate;

    private int currentIndex = 0;
    private bool isResetting = false;

    public void PressButton(LaserButton button)
    {
        if (isResetting) return;

        if (correctOrder[currentIndex] == button)
        {
            // ✅ Correct button
            button.SetCorrect();

            currentIndex++;

            if (currentIndex >= correctOrder.Length)
            {
                // 🎉 Puzzle complete
                if (linkedGate != null)
                {
                    linkedGate.DisableLaser();
                }
            }
        }
        else
        {
            // ❌ Wrong → flash then reset
            button.PlayWrongFeedback();
            StartCoroutine(ResetAfterFlash());
        }
    }

    private IEnumerator ResetAfterFlash()
    {
        isResetting = true;
        yield return new WaitForSeconds(0.2f);
        ResetPuzzle();
        isResetting = false;
    }

    void ResetPuzzle()
    {
        currentIndex = 0;

        foreach (var btn in correctOrder)
        {
            btn.ResetButton();
        }
    }
}