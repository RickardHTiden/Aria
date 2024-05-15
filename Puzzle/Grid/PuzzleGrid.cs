using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]
public class PuzzleGrid : MonoBehaviour {

    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject startNodePrefab;

    [SerializeField] private int size;
    [SerializeField] private VisualEffect currentNodeEffect;
    [SerializeField] private string solution;

    private int nodeOffset = 3;
    
    //Grid components
    private List<Node> walkableNodes = new List<Node>();
    private List<Node> lineNodes = new List<Node>();
    private Stack<LineObject> lineRenderers = new Stack<LineObject>();
    private Node currentNode;
    private Node startNode;

    

    public Node[,] allNodes { get; private set; }

    private PuzzleLine currentLine;
    private GameObject currentLineObject;

    public int NodeOffset => nodeOffset;
    public int Size => size;
    public Transform Player { get; set; }

    private Puzzle masterPuzzle;

    public string GetSolution() 
    {
        if (solution.Length > 0)
            return solution;
        else
            return "";
    }
    
    private void Update()
    {
        if(currentLine != null)
        {
            currentLine.LineErasable(true);
            currentLine.Play();
            currentLine.transform.position = currentNode.transform.position;
            currentLine.transform.localRotation = Quaternion.Inverse(GetComponentInParent<Puzzle>().transform.rotation);
            currentLine.SetPosition((new Vector3(Player.position.x,currentLine.transform.position.y, Player.position.z) - currentLine.transform.position));
        }
    }

    private void Start()
    {
        (GameMenuController.Instance.RequestOption<CurrentNodeMarker>() as CurrentNodeMarker).AddListener(ApplySettings);
    }

    private void OnDisable()
    {
        (GameMenuController.Instance.RequestOption<CurrentNodeMarker>() as CurrentNodeMarker).RemoveListener(ApplySettings);
    }

    private void ApplySettings(bool showCurrentNodeMarker)
    {
        if (showCurrentNodeMarker)
            currentNodeEffect.Play();
        else
            currentNodeEffect.Stop();
    }


    //Setup puzzle from Puzzle
    public void StartGrid()
    {

        GenerateGrid();

        masterPuzzle = GetComponentInParent<Puzzle>();
        //allNodesLIST.AddRange(transform.GetComponentsInChildren<Node>());
        foreach (Node node in allNodes)
            node.gameObject.SetActive(false);

        currentNodeEffect.Play();
        currentNodeEffect.transform.localPosition = startNode.transform.localPosition;
        startNode.gameObject.SetActive(true);
    }

    public List<Node> FindNeighbours(Node n)
    {

        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int nX = n.PosX + x;
                int nY = n.PosY + y;

                if (nX >= 0 && nX < size && nY >= 0 && nY < size)
                {
                    neighbours.Add(allNodes[nX, nY]);
                }
            }
        }
        
        return neighbours;
    }


    void GenerateGrid()
    {
        allNodes = new Node[size, size];
        int midIndex = size / 2;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if(x == midIndex && y == midIndex)
                    allNodes[x, y] = Instantiate(startNodePrefab, transform).GetComponent<Node>();
                else
                    allNodes[x, y] = Instantiate(nodePrefab, transform).GetComponent<Node>();

                allNodes[x, y].PosX = x;
                allNodes[x, y].PosY = y;
                allNodes[x, y].gameObject.name = "" + x + "|" + y;
                allNodes[x, y].transform.position = transform.position;
                allNodes[x, y].transform.localPosition = (x * Vector3.right * nodeOffset) + (y * Vector3.forward * nodeOffset);
                allNodes[x, y].grid = this;

                
            }
        }

        foreach(Node n in allNodes)
        {
            n.SetNeighbours(FindNeighbours(n));
        }

        allNodes[midIndex, midIndex].SetStartNode();
        startNode = currentNode = allNodes[midIndex, midIndex];
        transform.localPosition = (Vector3.right * -midIndex * nodeOffset) + (Vector3.forward * -midIndex * nodeOffset);
        
        //Move up the grid
        transform.localPosition += Vector3.up * 0.5f;
    }

    public void StartPuzzle()
    {
        ActivateNode(startNode, false);
        InstantiateFirstLine();
    }

    private void InstantiateFirstLine()
    {
        //instansiera linje
        //rita linje från startnod till spelare
        currentLineObject = Instantiate(linePrefab, transform.parent);
        currentLine = currentLineObject.GetComponent<PuzzleLine>();      
    }

    internal void SetRestrictions(List<Vector2Int> unrestricted)
    {
        int midIndex = size / 2;
        foreach (Node n in allNodes)
            n.Drawable = false;
        
        foreach(Vector2Int vInt in unrestricted)
        {
            allNodes[vInt.x + midIndex, vInt.y + midIndex].Drawable = true;
        }

        startNode.Drawable = true;
    }

    public void AddSelectedNode(Node node) 
    {
        if (node == currentNode || !currentNode.neighbours.ContainsKey(node))
            return;

        LineObject newLine = new LineObject(node);

        #region ERASER
        //If a line already exists with currentNode, erase
        if (lineRenderers.Count > 0 && lineRenderers.Peek().CompareLastLine(newLine))
        {
            //Hazard
            EventHandler<UpdateHazardEvent>.FireEvent(new UpdateHazardEvent(true));
            EraseLine(node);
            return;
        }
        #endregion

        #region CREATE_NEW_LINE
        //create new Line
        else
        {
            //Unless that line already exists
            if (lineRenderers.Count > 1)
            {
                //Checks if there exists a line between these nodes already, if so it destroys the line that was created
                if (node.HasLineToNode(currentNode))
                {
                    return;
                }
            }

            //Line Instantiation
            CreateNewLine(node);

        }
        #endregion

        #region ADD_TO_SOLUTION

        solution += PuzzleHelper.TranslateLocalInput(node, currentNode);

        //Every time we collide with node. Check if we have solution
        if (SendToPuzzleForEvaluation())
        {   
            TurnOffCollision();
            return;
        }

        #endregion

        #region MOVE_CURRENT_NODE
        
        currentNode = node;
        lineNodes.Add(currentNode);
        ActivateNode(node, false);

        #endregion

        EventHandler<UpdateHazardEvent>.FireEvent(new UpdateHazardEvent(false));


    }

    private void CreateNewLine(Node node)
    {

        if (currentLine == null)
            return;

        if(lineRenderers.Count > 0)
            lineRenderers.Peek().ErasableLine(false);

        //Instantiate Line
        GameObject newLineRenderer = Instantiate(linePrefab, transform);
        newLineRenderer.transform.position = currentNode.transform.position;
        newLineRenderer.transform.localRotation = Quaternion.Inverse(masterPuzzle.transform.rotation);


        //Set position of the particle system
        currentLine.GetComponent<PuzzleLine>().SetPosition((
            node.transform.localPosition - currentNode.transform.localPosition).normalized *
            Vector3.Distance(node.transform.localPosition, currentNode.transform.localPosition),
            masterPuzzle.transform.rotation);

        LineObject line = new LineObject(currentNode, currentLineObject);

        currentLine = newLineRenderer.GetComponent<PuzzleLine>();
        currentLineObject = newLineRenderer;

        //ADD Line to neighbourList and list of lines
        node.AddLineToNode(currentNode);
        currentNode.AddLineToNode(node);
        lineRenderers.Push(line);
        lineRenderers.Peek().ErasableLine(true);
    }

    private void EraseLine(Node node)
    {
  
        //Checks if this was the last line that was drawn, if so delete that line (eraser)
        LineObject oldLine = lineRenderers.Pop();
        foreach (Node n in currentNode.neighbours.Keys)
        {
            n.RemoveEnablingNode(currentNode);
        }

        /*
        if (lineNodes.Contains(currentNode))
            lineNodes.Remove(currentNode);
        */

        //REMOVE LAST CHAR IN SOLUTION
        solution = PuzzleHelper.RemoveLastChar(solution);

        node.RemoveLineToNode(currentNode);
        currentNode.RemoveLineToNode(node);
        currentNode = node;
        currentNodeEffect.transform.localPosition = currentNode.transform.localPosition;
        ActivateNode(node, true);
        Destroy(oldLine.line);

        if (lineRenderers.Count > 0)
            lineRenderers.Peek().ErasableLine(true);


        SendToPuzzleForEvaluation();
    }

    private void TurnOffCollision()
    {
        foreach (Node n in allNodes)
        {
            n.TurnOffCollider();
        }
    }

    private bool SendToPuzzleForEvaluation()
    {
        masterPuzzle.CheckIfClearedSymbol(solution);
        if (masterPuzzle.EvaluateSolution())
        {   
            DestroyCurrentLine();
            return true;
        }

        return false;
    }

    

    //Show and activate neighbours
    private void ActivateNode(Node node, bool eraser) 
    {
        node.HitNode();

        node.gameObject.SetActive(true);

        currentNodeEffect.transform.localPosition = currentNode.transform.localPosition;

        if(walkableNodes.Count > 0)
        {
            foreach(Node n in walkableNodes)
            {
                n.Walkable(false);
            }
            walkableNodes.Clear();
        }
        

        foreach (Node neighbour in node.neighbours.Keys) {

            
            if (neighbour.Drawable == false)
                continue;
            
            //SHOW THE NODES THAT YOU CAN WALK TO 
            neighbour.gameObject.SetActive(true);
            neighbour.TurnOnCollider();
            if(!eraser)
                neighbour.enabledBy.Add(node);

            neighbour.OnNodeSelected += AddSelectedNode;

            neighbour.Walkable(true);
            walkableNodes.Add(neighbour);
        }

        //MARK THE NODES YOU CANT WALK TO
    }

    #region TURNING_OFF_GRID

    #region RESET_GRID
    public void ResetGrid()
    {
        TurnOffLines();
        TurnOffNodes();

        solution = "";

        currentNodeEffect.transform.localPosition = startNode.transform.localPosition;

        DestroyCurrentLine();

        EventHandler<ResetHazardEvent>.FireEvent(new ResetHazardEvent());

    }

    private void TurnOffLines()
    {
        foreach (LineObject line in lineRenderers)
        {
            line.line.GetComponent<PuzzleLine>().TurnOffLine();
        }
        lineRenderers.Clear();
    }

    private void TurnOffNodes()
    {
        foreach (Node n in allNodes)
        {
            if (n.startNode == false)
            {
                n.ResetNeighbours();
                n.TurnOffCollider();
                n.TurnOff();
                n.Drawable = true;
            }
            else
                n.TurnOffCollider();
        }
        //RestartStartNode();
        Invoke("RestartStartNode", 1f);
    }

    private void RestartStartNode()
    {
        currentNode = startNode;
        currentNode.TurnOnCollider();
        currentNode.ResetNeighbours();
        Invoke("TellPuzzleGridIsReady", 3f);
    }

    private void TellPuzzleGridIsReady()
    {
        masterPuzzle.InitiatePuzzle();
        masterPuzzle.RegisterToResetPuzzleEvent();
    }

    private void DestroyCurrentLine()
    {
        if (currentLine != null)
        {
            currentLine.Stop();
            Destroy(currentLine.gameObject);
            currentLine = null;
        }
    }

    #endregion

    #region COMPLETE_GRID

    public void CompleteGrid()
    {
        DestroyNodes();
        DestroyLines();
        currentNodeEffect.Stop();
    }

    private void DestroyLines()
    {
        foreach (LineObject line in lineRenderers)
        {
            line.line.GetComponent<PuzzleLine>().TurnOffLine();
        } 
    }

    private void DestroyNodes()
    {
        foreach (Node n in allNodes)
        {
            n.TurnOffCollider();
            n.TurnOff();
            Destroy(n.gameObject, 2);
        }
    }
    #endregion


    #endregion
}

public class LineObject
{
    //Object that can compare lines between nodes, stored in a stack in the grid
    //THIS LINE OBJECT SHOULD HOLD THE NODES THAT WERE ENABLED
    public Node originNode;
    public GameObject line;

    public LineObject(Node a, GameObject lineRen)
    {
        originNode = a;
        line = lineRen;
    }
    public LineObject(Node a)
    {
        originNode = a;
    }
    
    public bool CompareLastLine(LineObject other)
    {
        return originNode == other.originNode;
    }

    public void ErasableLine(bool b)
    {
        line.GetComponent<PuzzleLine>().LineErasable(b);
    }
    

}