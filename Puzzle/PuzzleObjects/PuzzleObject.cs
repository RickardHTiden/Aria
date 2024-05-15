using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public abstract class PuzzleObject : MonoBehaviour
{
    [SerializeField] protected string translation;
    [SerializeField] private Vector3 modifierPosition;
    [SerializeField] private GameObject modHolder;

    [HideInInspector]
    [SerializeField] private ModifierVariant modVariant;

    [SerializeField]private MeshRenderer mesh;

    [SerializeField] private List<Material> materials_EASY_MEDIUM_HARD = new List<Material>();
    private Dictionary<string, Material> materialsByDifficulty = new Dictionary<string, Material>();
    

    private ModInfo modInfo;
    private Image modifierImage; //dekal som ska visas någonstans!?!? HUR GÖR MAN
    private GameObject modifier;
    private Animator anim;

    private FMOD.Studio.EventInstance SymbolClear;


    public bool Active { get; private set; }


    private void Awake() {
        anim = GetComponent<Animator>();
        SetUpMaterials();
    }

    private void Start()
    {
        (GameMenuController.Instance.RequestOption<SymbolDifficulty>() as SymbolDifficulty).AddListener(ApplyDifficulty);
        //EventHandler<RequestSettingsEvent>.FireEvent(null);
    }

    private void OnDisable()
    {
        if(GameMenuController.Instance != null)
            (GameMenuController.Instance.RequestOption<SymbolDifficulty>() as SymbolDifficulty).RemoveListener(ApplyDifficulty);
        //EventHandler<SaveSettingsEvent>.UnregisterListener(ApplyDifficulty);
    }
    private void ApplyDifficulty(int choice)
    {
        
        string difficulty = (GameMenuController.Instance.RequestOption<SymbolDifficulty>() as SymbolDifficulty).GetValue();
        
        if (materials_EASY_MEDIUM_HARD.Count > 0)
        {
            SetMaterialBasedOnDifficulty(difficulty);

            Debug.Log($"");
        }
    }
   
    /* private void ApplyDifficulty(SaveSettingsEvent obj)
    {  
        if (materials_EASY_MEDIUM_HARD.Count > 0)
        {
            SetMaterialBasedOnDifficulty(obj.settingsData.symbolDifficulty);
        }
    }*/

    private void SetUpMaterials()
    {
        if(materials_EASY_MEDIUM_HARD.Count > 0)
        {
            materialsByDifficulty.Add("Easy", materials_EASY_MEDIUM_HARD[0]);
            materialsByDifficulty.Add("Medium", materials_EASY_MEDIUM_HARD[1]);
            materialsByDifficulty.Add("Hard", materials_EASY_MEDIUM_HARD[2]);
        }
    }

    private void SetMaterialBasedOnDifficulty(string difficulty)
    {
        if (materials_EASY_MEDIUM_HARD.Count > 0) {
            
            mesh.material = materialsByDifficulty[difficulty];
        }
            
    }

    public string GetTranslation()
    {
        return AdjustForModifiers();
    }

    protected string AdjustForModifiers()
    {
        string modifiedString = "";
        
        if(modInfo != null)
        {
            modifiedString += modInfo.translation;
        }

        //CANNOT COMBINE MODIFIERS RIGHT NOW
        return modifiedString + translation;
    }


    public void SetModifier(ModifierVariant modVar)
    {
        if (modifier != null)
            Destroy(modifier);
        modInfo = modHolder.GetComponent<ModifierHolder>().GetModifier(modVar);
        modifier = Instantiate(modInfo.modifier);
        modifier.transform.parent = transform;
        modifier.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        modifier.transform.localPosition = modifierPosition;
        modifier.transform.rotation = transform.rotation;
    }

    internal void Unload()
    {
        Invoke("DestroyPuzzleObject", 2);
    }

    private void DestroyPuzzleObject()
    {
        anim.SetTrigger("off");

        if (modInfo.variant != ModifierVariant.None == true)
        {
            modifier.GetComponent<Animator>().SetTrigger("off");
        }
        Destroy(gameObject, 2);
    }
    internal void TurnOn()
    {
        Debug.Log(gameObject + " ON ");
    }
    internal void TurnOff()
    {
        Debug.Log(gameObject + " OFF ");
    }

    internal void Activate(bool hasBeenSolved)
    {
        if(Active != hasBeenSolved)
        {
            //Debug.Log(gameObject + "  " + hasBeenSolved);
            Active = hasBeenSolved;

            if(hasBeenSolved == true)
            {
                anim.SetTrigger("activate");

                SymbolClear = FMODUnity.RuntimeManager.CreateInstance("event:/Game/Puzzle/SymbolClear");
                SymbolClear.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                SymbolClear.start();
                SymbolClear.release();

                if (modInfo.variant != ModifierVariant.None == true)
                {
                    modifier.GetComponent<Animator>().SetTrigger("activate");
                }
            }
            else
            {
                anim.SetTrigger("deactivate");

                if (modInfo.variant != ModifierVariant.None == true)
                {
                    modifier.GetComponent<Animator>().SetTrigger("deactivate");
                }
            }
            
        }

        

    }
}

public enum ModifierVariant
{
    None,
    Mirrored, 
    Double,
    Rotate90,
    Rotate180,
    Rotate270,
    RepeatOPEN,
    RepeatCLOSE
}
