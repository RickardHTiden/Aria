using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Node : MonoBehaviour {

    [SerializeField] private LayerMask nodeLayer;

    
    public delegate void OnSelected(Node node);
    public event OnSelected OnNodeSelected;



    //MAKE PRIVATE
    public Dictionary<Node, bool> neighbours { get; private set; }
    public List<Node> enabledBy = new List<Node>();
    //public List<Node> enabledNodes = new List<Node>(); // this can be in LineObject instead so that a LINE knows what nodes it lit up
    
    public bool startNode;

    public int PosX, PosY;

    public bool Drawable { get; set; }


    public PuzzleGrid grid;

    private VisualEffect hitEffect;
    private Animator anim;
    private SphereCollider coll;
    private float colliderRadius = 1.2f;
    private Vector3 size = new Vector3(0.4f, 0.4f, 0.4f);

    static private int sizeMultiplier = 1;
    private void Awake() {
        hitEffect = GetComponent<VisualEffect>();
        anim = GetComponent<Animator>();
        coll = GetComponent<SphereCollider>();
        neighbours = new Dictionary<Node, bool>();
        Drawable = true;
        TurnOn();
        //FindNeighbours();
        //PosX = transform.localPosition.x;
        //PosY = transform.localPosition.y;
    }



    private void SetSizeMultiplier(bool b)
    {
        if (b)
            sizeMultiplier = 2;
        else
            sizeMultiplier = 1; 
    }

    private void SetSize()
    {
        transform.localScale = size * sizeMultiplier;
        coll.radius = colliderRadius / sizeMultiplier;
    }

    private void OnEnable()
    {
        SetSize();
    }


    private void ApplySettings(bool isBig)
    {
        SetSizeMultiplier(isBig);
        SetSize();
    }
    private void Start()
    {
        (GameMenuController.Instance.RequestOption<BigNodes>() as BigNodes).AddListener(ApplySettings);
        gameObject.SetActive(true);
        TurnOn();
    }

    private void OnDestroy()
    {
        (GameMenuController.Instance.RequestOption<BigNodes>() as BigNodes).RemoveListener(ApplySettings);
    }


    private void OnTriggerEnter(Collider other)
    {
        grid.AddSelectedNode(this);
    }

    public void HitNode()
    {
        hitEffect.Play();
    }

    private void FindNeighbours() {
        float angle = 0; 
        
        for (int i = 0; i < 8; i++) {

            Vector3 direction = transform.parent.rotation * new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad),0);
            
            Physics.Raycast(transform.position, direction, out var hit, 5, nodeLayer);

            //Debug.DrawRay(transform.position, direction * 5, Color.cyan, 10);

            if(hit.collider)
                neighbours.Add(hit.transform.GetComponent<Node>(), false);

            angle += 45f;
            
        }
    }

    public Node FindSpecificNeighbour(Vector3 direction)
    {
        Physics.Raycast(transform.position, direction, out var hit, 5, nodeLayer);

        if (hit.collider)
        {
            return hit.transform.GetComponent<Node>();
        }

        return null;
    }

    public void AddLineToNode(Node n)
    {
        neighbours[n] = true;
    }

    public void RemoveLineToNode(Node n)
    {
        neighbours[n] = false;
    }

    public void ResetNeighbours()
    {
        foreach (var key in neighbours.Keys.ToList())
        {
            neighbours[key] = false;
        }
    }

    //Currently doesnt know if the node actually is a neighbour, only checks if there is a line drawn to it - 
    // meaning you can draw to nodes that are not neighbours
    public bool HasLineToNode(Node n)
    {
        return neighbours.ContainsKey(n) ? neighbours[n] : false;
    }

    public void TurnOffCollider()
    {
        GetComponent<SphereCollider>().enabled = false;
    }

    public void ClearSelectable() => OnNodeSelected = null;

    public void TurnOnCollider()
    {
        GetComponent<SphereCollider>().enabled = true;
    }

    public void SetStartNode()
    {
        startNode = true;
    }

    public void SetNeighbours(List<Node> list)
    {
        foreach(Node n in list)
        {
            neighbours.Add(n, false);
        }
    }

    public void TurnOn()
    {
        anim.SetBool("On", true);
        
    }

    internal void TurnOff()
    {
        //Animate Shit
        anim.SetBool("Off", true);
        enabledBy.Clear();
        
        //gameObject.SetActive(false);
    }

    public void AnimOff()
    {
        anim.SetBool("Off", false);
    }
    public void AnimOn()
    {
        anim.SetBool("On", false);
    }

    public void TurnOffGameObject()
    {
        if (startNode == false)
            gameObject.SetActive(false);
    }

    internal void Restart()
    {
        Invoke("TurnOn", 1);
    }

    internal void RemoveEnablingNode(Node currentNode)
    {
        enabledBy.Remove(currentNode);
        if (enabledBy.Count == 0 && startNode == false)
            TurnOff();
    }

    public void Walkable(bool isWalkable)
    {
        anim.SetBool("Walkable",isWalkable);
    }
}

[Serializable] public class DictionaryOfIntAndBool : SerializableDictionary<int, bool> { }
