using System.Collections;
using UnityEngine;

public class BarricadeSequence : MonoBehaviour
{
    [Header("Barricade Rows (2 per row)")]
    public Barricade[] row1;
    public Barricade[] row2;
    public Barricade[] row3;
    public Barricade[] row4;

    [Header("Timing")]
    public float delayBetweenRows = 0.5f;

    private bool hasActivated = false;

    public void StartSequence()
    {
        if (hasActivated) return;

        hasActivated = true;
        StartCoroutine(SequenceRoutine());
    }

    IEnumerator SequenceRoutine()
    {
        ActivateRow(row1);
        yield return new WaitForSeconds(delayBetweenRows);

        ActivateRow(row2);
        yield return new WaitForSeconds(delayBetweenRows);

        ActivateRow(row3);
        yield return new WaitForSeconds(delayBetweenRows);

        ActivateRow(row4);
    }

    void ActivateRow(Barricade[] row)
    {
        foreach (var barricade in row)
        {
            if (barricade != null)
                barricade.Activate();
        }
    }
}