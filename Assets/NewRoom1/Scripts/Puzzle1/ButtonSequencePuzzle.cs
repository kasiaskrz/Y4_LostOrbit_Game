using UnityEngine;

public class ButtonSequencePuzzle : MonoBehaviour
{
    [Header("All 5 buttons in the puzzle (for resetting)")]
    public SimpleButton[] allButtons;     // MUST be size 5

    [Header("Correct order (size 5): C -> E -> A -> B -> D")]
    public SimpleButton[] correctOrder;   // MUST be size 5

    SimpleButton[] entered = new SimpleButton[5];
    int step = 0;
    bool solved = false;

    public void OnButtonPressed(SimpleButton btn)
    {
        if (solved) return;
        if (btn == null) return;

        // Hard validation: if you didn’t set arrays correctly, you’ll see it instantly.
        if (!IsSetupValid())
        {
            Debug.LogError("ButtonSequencePuzzle setup invalid. Fix Inspector arrays (allButtons=5, correctOrder=5, no nulls).");
            return;
        }

        // record what player pressed (1..5)
        entered[step] = btn;
        step++;

        // only check AFTER 5 presses
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
            // leave green
        }
        else
        {
            ResetAllInstant();
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

    void ResetAllInstant()
    {
        // reset visuals + allow pressing again
        for (int i = 0; i < 5; i++)
            allButtons[i].ResetButton();

        // reset attempt
        for (int i = 0; i < 5; i++)
            entered[i] = null;

        step = 0;
    }
}
