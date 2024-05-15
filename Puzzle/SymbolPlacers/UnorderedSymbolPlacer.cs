using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnorderedSymbolPlacer : SymbolPlacer
{
    //randomizing starting offset between these 2 values, for visual variation
    [SerializeField]private Vector2 offsetMinMax = new Vector2(45, 90);
    private List<PuzzleObject> instantiatedSymbolsClone;

    //Symbol placement
    [SerializeField] private float panelSize = 2f;
    private float degreeOffset;
    private float startingOffset;
    private float panelPlacementDepthOffset = 0.1f;

    protected override void UnevenPlaceSymbols()
    {
        //Should probably only relate to the correct number of positions (if we have predefined acceptable positions for placement), uneven or even shouldnt matter.
        CalculateSymbolPosition();
        EvenPlaceSymbols();
    }

    private void CalculateSymbolPosition()
    {
        Physics.Raycast(transform.position + Vector3.up * 2, Vector3.forward, out RaycastHit panelHit);
        Quaternion.Euler(panelHit.normal);
    }

    protected override void EvenPlaceSymbols()
    {
        startingOffset = Random.Range(offsetMinMax.x, offsetMinMax.y);
        //May be a way to handle this depending on implemenetation, discarding extra symbols or extra positions for instance

        instantiatedSymbolsClone = new List<PuzzleObject>(instantiatedSymbols);
        //might not be what we want to do here, actually
        degreeOffset = Mathf.Deg2Rad * (360 / instantiatedSymbolsClone.Count);        
        int counter = 1;     
        
        for (int i = instantiatedSymbolsClone.Count; i > 0; i--)
        {
            float radians = startingOffset + counter * degreeOffset;
            //this does not consider symbolPos rotation?

            PlaceSymbols(symbolPos.position + (symbolPos.rotation * new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * panelSize));
            counter++;
        } 

    }

    private void PlaceSymbols(Vector3 newPos)
    {
        //Move and REmove instance from list
        PuzzleObject instance = instantiatedSymbolsClone[Random.Range(0, instantiatedSymbolsClone.Count)];       

        instance.transform.position = newPos;
        instance.transform.rotation = symbolPos.rotation;

        Vector3 pos = new Vector3(instance.transform.localPosition.x, instance.transform.localPosition.y, panelPlacementDepthOffset);
        instance.transform.localPosition = pos;

        instantiatedSymbolsClone.Remove(instance);
    }

}
