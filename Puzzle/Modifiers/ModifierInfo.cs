using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Puzzle/ModifierInfo")]
public class ModifierInfo : ScriptableObject
{
    
    public Sprite Image; //modifier image to display
    public char ModifierTranslation; //typ 'Q' eller 'R'
}
