using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class PuzzleTranslator
{

    
    private string solution;
    protected List<TranslationAndObject> translations = new List<TranslationAndObject>();

   

    public string CalculateSolution(List<PuzzleObject> objects)
    {
        solution = "";
        translations.Clear();

        //bygg först en array med alla symbolers översättningar. så streck upp blir bara en 8 t.ex
        foreach(PuzzleObject obj in objects)
        {
            TranslationAndObject newPair = new TranslationAndObject();
            newPair.translation = obj.GetTranslation();
            newPair.pObj = obj;
            translations.Add(newPair);
        }

        //gå igenom array och översätt med switchcase t.ex. logiska operatorer blir något annat här t.ex. case R så roteras den
        for (int i = 0; i < translations.Count; i++)
        {
            string newString = "";
            string oldString = translations[i].translation;
            StringBuilder sb = new StringBuilder();

            switch (translations[i].translation[0])
            {
                case 'Q':
                    newString = PuzzleHelper.SkipFirstChar(oldString);
                    translations[i].translation = PuzzleHelper.RotateSymbolsTwoStep(newString);
                    break;

                case 'W':
                    newString = PuzzleHelper.SkipFirstChar(oldString);
                    for(int j = 0; j < 2; j++)
                    {
                        newString = PuzzleHelper.RotateSymbolsTwoStep(newString);
                    }
                    translations[i].translation = newString;
                    break;

                case 'E':
                    newString = PuzzleHelper.SkipFirstChar(oldString);
                    for (int j = 0; j < 3; j++)
                    {
                        newString = PuzzleHelper.RotateSymbolsTwoStep(newString);
                    }
                    translations[i].translation = newString;
                    break;
                    /*
                case 'R':
                    newString = PuzzleHelper.SkipFirstChar(oldString);
                    translations[i].translation = newString;
                    TranslationAndObject newPair = new TranslationAndObject();
                    newPair.translation = obj.GetTranslation();
                    newPair.pObj = translations[i].pObj;
                    translations.Insert(i + 1, newString);
                    i++;
                    break;
                    */
                case 'X':
                    translations.RemoveAt(i);
                    break;

                case 'M':
                    newString = PuzzleHelper.SkipFirstChar(oldString);
                    translations[i].translation = PuzzleHelper.MirrorSymbols(newString);
                    break;
                case 'D':
                    newString = PuzzleHelper.SkipFirstChar(oldString);
                    translations[i].translation = PuzzleHelper.DoubleStrokes(newString);
                    break;
                    
                default:
                    break;
            }

        }

        /*
        for (int i = 0; i < translations.Count; i++)
        {
            switch (translations[i])
            {
                case "Q":
                    translations[i - 1] = PuzzleHelper.RotateSymbols(translations[i - 1]);
                    translations.Remove("Q");
                    i--;
                    break;
                case "W":

                    translations[i - 1] = PuzzleHelper.RotateSymbols(translations[i - 1]);
                    translations.Remove("W");
                    i--;
                    break;
                case "E":
                    translations[i - 1] = PuzzleHelper.RotateSymbols(translations[i - 1]);
                    translations.Remove("E");
                    i--;
                    break;
                case "R":
                    translations[i] = translations[i - 1];
                    break;

                default:
                    break;
            }
        }
        */

        foreach (TranslationAndObject to in translations)
        {
            solution += to.translation;
        }

        return solution;
        #region TRANSLATIONS
        /*
         * 8 = N
         * 9 = NE
         * 6 = E
         * 3 = SE
         * 2 = S
         * 1 = SW
         * 4 = W
         * 7 = NW
         * Q = Rotate
         * R = Repeat
         * { = Loop open, save the following until close
         * } = Loop close, repeat all the saved translations
         */
        #endregion
    }

    internal List<TranslationAndObject> GetTranslations()
    {
        return translations;
    }



    /*
    private void FindLoop(int index)
    {
        for(int i = index; i < translations.Count; i++)
        {
            if (translations[i] != "}")
            {
                loopSymbols.Add(translations[i]);
            }
            else
                break;
        }
    }
    */



}


