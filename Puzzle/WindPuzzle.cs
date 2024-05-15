using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WindPuzzle : Puzzle
{

    [SerializeField] private VisualEffect wind;
    [SerializeField] private Animator windBurst;

  

    public override void InitiatePuzzle()
    {

        //Start Wind
        wind.Play();
        wind.SetVector3("Direction", currentPuzzleInstance.GetComponent<WindPuzzleInstance>().GetWindDirection());
        windBurst.SetTrigger("burst");
        base.InitiatePuzzle();

        //adjust solution for wind
        string newString = solution;
        //Debug.Log(currentPuzzleInstance.GetComponent<WindPuzzleInstance>().GetWindRotations());
        for (int j = 0; j < currentPuzzleInstance.GetComponent<WindPuzzleInstance>().GetWindRotations(); j++)
        {
            newString = PuzzleHelper.RotateSymbolsOneStep(newString);
            for(int i = 0; i < translations.Count; i++)
            {
                translations[i].translation = PuzzleHelper.RotateSymbolsOneStep(translations[i].translation);
            }
        }

        solution = newString;
        //Debug.Log("THE SOLUTION IS : " + solution);
    }

    public override bool EvaluateSolution()
    {

        if (solution.Equals(grid.GetSolution()))
        {
            
            currentPuzzleInstance.Solve();
            EventHandler<SaveEvent>.FireEvent(new SaveEvent());
            EventHandler<ClearPuzzleEvent>.FireEvent(new ClearPuzzleEvent(new PuzzleInfo(GetPuzzleID())));

            NextPuzzle();
            return true;
        }

        return false;
    }

    protected override void NextPuzzle()
    {
        wind.Stop();
        base.NextPuzzle();
    }
}
