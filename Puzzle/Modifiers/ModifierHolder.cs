using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CreateAssetMenu(menuName = "Puzzle/ObjectModifierHolder")]
public class ModifierHolder : MonoBehaviour
{

    [SerializeField] private List<ModInfo> modifiers = new List<ModInfo>();

    public ModInfo GetModifier(ModifierVariant var) 
    { 
        foreach(ModInfo kv in modifiers)
        {
            if (kv.variant == var)
                return kv;
        }

        return null;
    }

}

[System.Serializable]
public class ModInfo
{
    public ModifierVariant variant;
    public string translation;
    public GameObject modifier;

}



