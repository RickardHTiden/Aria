using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleInstance : MonoBehaviour
{
    [SerializeField] private int puzzleID; //should be compared to solution on a EvaluatePuzzleEvent and fire a SUCCESS EVENT or FAIL EVENT
    [SerializeField] public List<SymbolModPair> puzzleObjects = new List<SymbolModPair>();

    [SerializeField] private List<Vector2Int> activeNodes;
    [SerializeField] private List<Hazard> hazards;

    private bool currentState;
    private List<Hazard> instantiatedHazards = new List<Hazard>();

    public string puzzleDescription;

    private FMOD.Studio.EventInstance puzzleDescriptionSound;

    public bool IsSolved() => currentState;

    private void Awake()
    {
        PuzzleDictionary.AddPuzzle(puzzleID);
    }

    public void SetupPuzzleInstance(Puzzle puzzle, int masterPuzzleID)
    {
        puzzleID = int.Parse(masterPuzzleID.ToString() + puzzleID.ToString());
    }
    public int GetPuzzleID()
    {
        return puzzleID;  
    }

    public void Solve() 
    { 
        currentState = true; 
        PuzzleDictionary.SetState(puzzleID, currentState); 
    }
    
    public void Load()
    {
        currentState = PuzzleDictionary.GetState(puzzleID);
        Debug.Log("Load puzzle " + puzzleID + "    STATE:: " + currentState);
    }


    public void SetUpHazards()
    {
        if(hazards.Count > 0)
        {
            if(instantiatedHazards.Count == 0)
            {
                foreach (Hazard h in hazards)
                {
                    Hazard instance = Instantiate(h, transform.position, new Quaternion(0,0,0,0), transform).GetComponent<Hazard>();
                    instantiatedHazards.Add(instance);
                    instance.StartHazard(GetPuzzleID());
                }
            }
        }
    }

    public void DestroyHazards()
    {
        if (hazards.Count > 0)
        {
            foreach (Hazard h in instantiatedHazards)
            {
                h.DeleteHazardObjects();
                Destroy(h.gameObject);
            }
            instantiatedHazards.Clear();
        }
    }
  

    public bool HasRestrictions()
    {
        return activeNodes.Count > 0;
    }
    public List<Vector2Int> GetRestrictions()
    {
        return activeNodes;
    }

    internal void PlayDescription()
    {
        Debug.Log("Play Puzzle Sound");
        puzzleDescriptionSound = FMODUnity.RuntimeManager.CreateInstance("event:/puzzleDescription/" + puzzleDescription);
        puzzleDescriptionSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        puzzleDescriptionSound.start();
        puzzleDescriptionSound.release();
        //play puzzleDescription;
    }
}

[System.Serializable]
public class SymbolModPair
{
    public PuzzleObject symbol;
    public ModifierVariant modifier;
    
}
