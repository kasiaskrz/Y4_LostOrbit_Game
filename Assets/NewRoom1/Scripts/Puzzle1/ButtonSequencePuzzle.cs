using UnityEngine;
using System.Collections;

public class ButtonSequencePuzzle : MonoBehaviour
{
    [Header("All 5 buttons in the puzzle (for resetting)")]
    public SimpleButton[] allButtons;

    [Header("Correct order (size 5): C -> E -> A -> B -> D")]
    public SimpleButton[] correctOrder;

    public float wrongDelay = 1f;

    SimpleButton[] entered = new SimpleButton[5];
    int step = 0;
    bool solved = false;

    public void OnButtonPressed(SimpleButton btn)
    {
        if (solved) return;
        if (btn == null) return;

        if (!IsSetupValid())
        {
            Debug.LogError("ButtonSequencePuzzle setup invalid. Fix Inspector arrays.");
            return;
        }

        entered[step] = btn;
        step++;

        if (step < 5) return;

        bool correct = true;

        for (int i = 0; i < 5; i++)
        {
            if (entered[i] != correctOrder[i])
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            solved = true;
            Debug.Log("Solved! (Later: open door here)");
        }
        else
        {
            StartCoroutine(WrongSequence());
        }
    }

    bool IsSetupValid()
    {
        if (allButtons == null || allButtons.Length != 5) return false;
        if (correctOrder == null || correctOrder.Length != 5) return false;

        for (int i = 0; i < 5; i++)
        {
            if (allButtons[i] == null) return false;
            if (correctOrder[i] == null) return false;
        }
        return true;
    }

    IEnumerator WrongSequence()
    {
        // turn all buttons red
        for (int i = 0; i < 5; i++)
            allButtons[i].SetLightColor(Color.red);

        yield return new WaitForSeconds(wrongDelay);

        ResetAllInstant();
    }

    void ResetAllInstant()
    {
        for (int i = 0; i < 5; i++)
            allButtons[i].ResetButton();

        for (int i = 0; i < 5; i++)
            entered[i] = null;

        step = 0;
    }
}