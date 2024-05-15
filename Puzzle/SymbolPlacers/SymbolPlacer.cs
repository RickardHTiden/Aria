using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SymbolPlacer : MonoBehaviour
{
    [SerializeField] protected List<PuzzleObject> instantiatedSymbols = new List<PuzzleObject>();
    [SerializeField] protected float symbolOffset = 3f;
    protected Transform symbolPos;
    public List<PuzzleObject> PlaceSymbols(PuzzleInstance currentPuzzleInstance, Transform symbolPos)
    {
        this.symbolPos = symbolPos;

        foreach (SymbolModPair pair in currentPuzzleInstance.puzzleObjects)
        {
            GameObject instance = Instantiate(pair.symbol).gameObject;

            PuzzleObject objectInstance = instance.GetComponent<PuzzleObject>();
            instantiatedSymbols.Add(objectInstance);
            objectInstance.transform.parent = symbolPos;
            objectInstance.SetModifier(pair.modifier);
        }

        if (instantiatedSymbols.Count % 2 == 0)
            EvenPlaceSymbols();
        else
            UnevenPlaceSymbols();

        //Do we neeed to return this list? 
        return instantiatedSymbols;
    }
    public void UnloadSymbols()
    {
        foreach (PuzzleObject po in instantiatedSymbols)
        {
            po.Unload();
        }
        instantiatedSymbols.Clear();
    }

    protected virtual void EvenPlaceSymbols() { }
    protected virtual void UnevenPlaceSymbols() { }



}
