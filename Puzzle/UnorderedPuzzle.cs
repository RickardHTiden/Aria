using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnorderedPuzzle : Puzzle
{
    [SerializeField]private List<TranslationAndObject> translationsSorted = new List<TranslationAndObject>();

    public override bool EvaluateSolution()
    {
        if (CheckWhichSymbolsAreCleared(grid.GetSolution()))
        {
            currentPuzzleInstance.Solve();
            EventHandler<SaveEvent>.FireEvent(new SaveEvent());
            EventHandler<ClearPuzzleEvent>.FireEvent(new ClearPuzzleEvent(new PuzzleInfo(GetPuzzleID())));

            NextPuzzle();
            return true;
        }
        return false;
    }
    public override void InitiatePuzzle()
    {
        base.InitiatePuzzle();
        SortTranslations(translations);
    }

    public override void CheckIfClearedSymbol(string currentSolution)
    {
        //nada
    }

    private bool CheckWhichSymbolsAreCleared(string currentSolution)
    {
        string currentSolutionCopy = currentSolution;

        foreach (TranslationAndObject pair in translationsSorted)
        {
            if (currentSolutionCopy.Contains(pair.translation))
            {
                pair.pObj.Activate(true);
                currentSolutionCopy = currentSolutionCopy.Remove(currentSolutionCopy.IndexOf(pair.translation), pair.translation.Length);
            }
            else
            {
                pair.pObj.Activate(false);
            }
            
        }

        

        if (currentSolution.Length == solution.Length && currentSolutionCopy.Length == 0)
        {
            return true;
        }
        return false;

    }

    private void SortTranslations(List<TranslationAndObject> listToSort)
    {
        listToSort.Sort((a, b) => b.translation.Length.CompareTo(a.translation.Length));
        translationsSorted = listToSort;
    }

}
