using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindPuzzleInstance : PuzzleInstance
{


    [SerializeField] private char windDirection; //8 = North, 9 = NE and so on...

    public Vector3 GetWindDirection()
    {
        return PuzzleHelper.TranslateNumToDirection(windDirection);
    }

    public int GetWindRotations()
    {
        
        return PuzzleHelper.TranslateWindToRotations(windDirection);
    }


}
