using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WirePuzzle : MonoBehaviour
{
    [Header("Pair Count")]
    public int pairCount = 6; // change this to add/remove pairs

    [Header("Nodes")]
    public WireNode[] leftNodes;
    public WireNode[] rightNodes;

    [Header("Wire Line Prefab")]
    public GameObject wireLinePrefab;
    public RectTransform wireContainer;

    [Header("Colors")]
    public Color[] wireColors = { Color.red, Color.cyan, Color.yellow, Color.green, new Color(1f, 0.5f, 0f), Color.magenta };

    [Header("UI")]
    public GameObject puzzlePanel;

    public System.Action OnSolved;
    public static bool IsOpen { get; private set; }

    private int[] solution;
    private int[] connections;
    private WireLine[] drawnLines;
    private int selectedLeft = -1;
    private bool solved = false;

    void Start()
    {
        InitArrays();
        GenerateSolution();
        ResetPuzzle();
    }

    void OnEnable()
    {
        IsOpen = true;
        Time.timeScale = 0f;
    }

    void OnDisable()
    {
        IsOpen = false;
        Time.timeScale = 1f;
    }

    void InitArrays()
    {
        solution = new int[pairCount];
        connections = new int[pairCount];
        drawnLines = new WireLine[pairCount];

        for (int i = 0; i < pairCount; i++)
            connections[i] = -1;
    }

    void GenerateSolution()
    {
        for (int i = 0; i < pairCount; i++)
        {
            solution[i] = -1; // default to no match
            for (int j = 0; j < pairCount; j++)
            {
                if (rightNodes[j] != null && leftNodes[i] != null)
                {
                    if (rightNodes[j].GetBaseColor() == leftNodes[i].GetBaseColor())
                    {
                        solution[i] = j;
                        break;
                    }
                }
            }

            if (solution[i] == -1)
                Debug.LogWarning($"LeftNode{i} has no matching color on the right side!");
        }
    }

    void ResetPuzzle()
    {
        InitArrays();
        selectedLeft = -1;
        solved = false;

        foreach (var n in leftNodes) if (n != null) n.Reset();
        foreach (var n in rightNodes) if (n != null) n.Reset();

        foreach (var l in drawnLines)
            if (l != null) Destroy(l.gameObject);

        drawnLines = new WireLine[pairCount];
    }

    public void OnLeftNodeClicked(int index)
    {
        if (solved) return;

        if (selectedLeft == index)
        {
            selectedLeft = -1;
            return;
        }

        selectedLeft = index;
    }

    public void OnRightNodeClicked(int index)
    {
        if (solved || selectedLeft == -1) return;

        // if this right node is already connected to a different left node, disconnect it
        for (int i = 0; i < pairCount; i++)
        {
            if (i != selectedLeft && connections[i] == index)
            {
                connections[i] = -1;
                if (drawnLines[i] != null)
                {
                    Destroy(drawnLines[i].gameObject);
                    drawnLines[i] = null;
                }
                break;
            }
        }

        // disconnect existing wire from selected left node
        if (connections[selectedLeft] != -1)
        {
            if (drawnLines[selectedLeft] != null)
            {
                Destroy(drawnLines[selectedLeft].gameObject);
                drawnLines[selectedLeft] = null;
            }
        }

        connections[selectedLeft] = index;

        Color col = wireColors[selectedLeft % wireColors.Length];
        DrawWire(selectedLeft, index, col);

        selectedLeft = -1;

        CheckSolution();
    }

    void DrawWire(int leftIdx, int rightIdx, Color color)
    {
        if (wireLinePrefab == null || wireContainer == null) return;

        GameObject go = Instantiate(wireLinePrefab, wireContainer);
        WireLine line = go.GetComponent<WireLine>();

        Vector2 from = WorldToCanvasPosition(leftNodes[leftIdx].GetComponent<RectTransform>());
        Vector2 to = WorldToCanvasPosition(rightNodes[rightIdx].GetComponent<RectTransform>());

        line.DrawLine(from, to, color);
        drawnLines[leftIdx] = line;
    }

    Vector2 WorldToCanvasPosition(RectTransform target)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            wireContainer, screenPoint, null, out Vector2 localPoint);
        return localPoint;
    }

    void CheckSolution()
    {
        for (int i = 0; i < pairCount; i++)
        {
            if (connections[i] != solution[i]) return;
        }

        solved = true;
        Debug.Log("Wire puzzle solved!");
        Invoke(nameof(TriggerSolved), 0.5f);
    }

    void TriggerSolved()
    {
        puzzlePanel.SetActive(false);
        OnSolved?.Invoke();
    }
}