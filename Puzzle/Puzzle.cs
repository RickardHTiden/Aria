using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [SerializeField] private int masterPuzzleID; 
    [SerializeField] private List<PuzzleInstance> puzzleInstances = new List<PuzzleInstance>();
    [SerializeField] private string playerInput = "";
    [SerializeField] protected string solution;
    [SerializeField] Transform symbolPos;
    
    protected PuzzleInstance currentPuzzleInstance;
    protected PuzzleTranslator translator = new PuzzleTranslator();
    protected List<TranslationAndObject> translations;

    protected PuzzleGrid grid;
    
    private SymbolPlacer symbolPlacer;



    //should NOT be public, but ModularHintSystem currently relies on this List
    private List<PuzzleObject> placedSymbols = new List<PuzzleObject>();
    [SerializeField] private List<TranslationAndObject> translationsSorted = new List<TranslationAndObject>();

    //track progress
    private PuzzleCounter puzzleCounter;
    private int numOfPuzzles;
    private int currentPuzzleNum = 0;

    private Transform player;
    private PuzzleParticles particles;

    public float NextPuzzleTimer { get; } = 2.5f;
    public List<PuzzleObject> PlacedSymbols { get => placedSymbols; set => placedSymbols = value; }

    public void SetPlayer(Transform t) { player = t; grid.Player = player; }

    public PuzzleGrid GetGrid() { return grid; }

    void Awake()
    {
        symbolPlacer = GetComponent<SymbolPlacer>();
        puzzleCounter = GetComponentInChildren<PuzzleCounter>();
        particles = GetComponentInChildren<PuzzleParticles>();
        if (puzzleInstances.Count > 0)
        {
            SetupPuzzleInstances();
            currentPuzzleInstance = puzzleInstances[0];
            numOfPuzzles = puzzleInstances.Count;
            puzzleCounter.GenerateMarkers(numOfPuzzles);
            grid = GetComponentInChildren<PuzzleGrid>();
            grid.StartGrid();
            
            InitiatePuzzle();
            //solution = Translate();
            
        }
        else
            Debug.LogWarning("NO PUZZLE INSTANCES IN PUZZLE");        
    }

    public void Load()
    {
        currentPuzzleNum = 0;
        currentPuzzleInstance = puzzleInstances[currentPuzzleNum];
        grid.ResetGrid();
        PlaceSymbols();
        CheckSolvedPuzzles();
    }

    private void CheckSolvedPuzzles()
    {
        while(currentPuzzleInstance.IsSolved() && 
            currentPuzzleNum + 1 <= puzzleInstances.Count)
        {
            NextPuzzle();
        }

    }

    public void GoToNextPuzzle() { if (currentPuzzleNum+1 <= puzzleInstances.Count) { Debug.Log("Pushing to next puzzle"); NextPuzzle(); } }
    private void Start()
    {
        (GameMenuController.Instance.RequestOption<ShowClearedSymbols>() as ShowClearedSymbols).AddListener(ApplySettings);
    }
    private void OnEnable()
    {
        EventHandler<ExitPuzzleEvent>.RegisterListener(OnExitPuzzle);
        EventHandler<ResetPuzzleEvent>.RegisterListener(OnResetPuzzle);
        EventHandler<StartPuzzleEvent>.RegisterListener(StartPuzzle);
        
    }
    private void OnDisable()
    {
        EventHandler<ExitPuzzleEvent>.UnregisterListener(OnExitPuzzle);
        EventHandler<ResetPuzzleEvent>.UnregisterListener(OnResetPuzzle);
        EventHandler<StartPuzzleEvent>.UnregisterListener(StartPuzzle);
        (GameMenuController.Instance.RequestOption<ShowClearedSymbols>() as ShowClearedSymbols).RemoveListener(ApplySettings);
    }

    private void ApplySettings(bool isShowClearedSymbols)
    {
        showClearedSymbols = isShowClearedSymbols;

        if(showClearedSymbols == false)
        {
            foreach(TranslationAndObject pair in translationsSorted)
            {
                pair.pObj.Activate(false);
            }
        }
    }

    private void SetupPuzzleInstances()
    {
        foreach (PuzzleInstance pi in puzzleInstances)
        {
            pi.SetupPuzzleInstance(this, masterPuzzleID);
        }

    }

    public virtual void InitiatePuzzle()
    {       
        EventHandler<LoadPuzzleEvent>.FireEvent(new LoadPuzzleEvent(new PuzzleInfo(GetPuzzleID())));
        
        GetComponentInChildren<PuzzleStarter>().ResetStarter();
        //grid.ResetGrid();

        currentPuzzleInstance.SetUpHazards();

        if (currentPuzzleInstance.HasRestrictions())
            grid.SetRestrictions(currentPuzzleInstance.GetRestrictions());

        PlaceSymbols();
        solution = Translate();
        translations = translator.GetTranslations();
    }

    protected virtual void NextPuzzle()
    {
        symbolPlacer.UnloadSymbols();
        currentPuzzleInstance.DestroyHazards();

        if (particles != null)
            particles.Play();

        puzzleCounter.MarkedAsSolved(currentPuzzleNum);
        currentPuzzleNum++;     

        if(currentPuzzleNum >= puzzleInstances.Count)
        {
            symbolPlacer.UnloadSymbols();
            EventHandler<ActivatorEvent>.FireEvent(new ActivatorEvent(new PuzzleInfo(masterPuzzleID)));
            puzzleCounter.DeleteMarkers();
            CompletePuzzle();
            return;
        }

        grid.ResetGrid();
        currentPuzzleInstance = puzzleInstances[currentPuzzleNum];
    }

    public void PlayPuzzleDescription()
    {
        currentPuzzleInstance.PlayDescription();
    }

    private void CompletePuzzle()
    {
        Invoke("CompleteGrid", 2);
        EventHandler<ExitPuzzleEvent>.FireEvent(new ExitPuzzleEvent(new PuzzleInfo(masterPuzzleID), true));
        GetComponent<Collider>().enabled = false;

    }

    private void CompleteGrid()
    {
        grid.CompleteGrid();
    }

    public void RemoveInput()
    {
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < playerInput.Length - 1; i++)
        {
            sb.Append(playerInput[i]);
        }
    }

    private string Translate()
    {
        if (PlacedSymbols.Count > 0)
            return translator.CalculateSolution(PlacedSymbols);
        else
        {
            Debug.LogWarning("SOLUTION EMPTY, NO INSTANTIATED SYMBOLS");
            return null;
        }         
    }
    public virtual bool EvaluateSolution()
    {

        if (solution.Equals(grid.GetSolution()))
        {
            //uppdaterar curr puzzle
            currentPuzzleInstance.Solve();
            EventHandler<SaveEvent>.FireEvent(new SaveEvent());
            EventHandler<ClearPuzzleEvent>.FireEvent(new ClearPuzzleEvent(new PuzzleInfo(GetPuzzleID())));

            NextPuzzle();
            return true;
        }

        return false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!player)
            return;

        PuzzleInfo info = new PuzzleInfo(currentPuzzleInstance.GetPuzzleID());
        EventHandler<ExitPuzzleEvent>.FireEvent(new ExitPuzzleEvent(info, false));
        GetComponentInChildren<PuzzleStarter>().ResetStarter();
    }

    public void StartPuzzle(StartPuzzleEvent eve)
    {
        if (eve.info.ID == GetPuzzleID())
        {
            grid.StartPuzzle();
        }
    }

    public string GetSolution()
    {
        return solution;
    }

    //Maybe return ID from current PuzzleInstance instead
    public int GetPuzzleID() { return currentPuzzleInstance.GetPuzzleID(); }
    
    protected List<bool> clearedSymbols = new List<bool>();
    protected bool showClearedSymbols;

    public virtual void CheckIfClearedSymbol(string currentSolution) //currentSolution = what the player has drawn
    {
        if(showClearedSymbols == false)
             return;

        int solutionOffset = 0;
        //Checks for empty solution
        if(currentSolution.Length > 0 == false)
        {
            foreach(var pair in translations)
            {
                pair.pObj.Activate(false);
            }
        }
        //goes through each index of strings in translations
        for (int i = 0; i < translations.Count; i++)
        {
            if (IsEqualRange(solutionOffset, translations[i].translation.Length, currentSolution, i))
            {
                solutionOffset += translations[i].translation.Length;
                translations[i].pObj.Activate(true);
            }
            else
            {
                translations[i].pObj.Activate(false);
            }
        }
    }


    private bool IsEqualRange(int offset, int length, string currentSolution, int translationIndex)
    {
        if (offset + length > currentSolution.Length)
            return false;

        return currentSolution.Substring(offset, length).Equals(translations[translationIndex].translation);
    }

    public void OnExitPuzzle(ExitPuzzleEvent eve)
    {
        GetComponent<SphereCollider>().enabled = false;

        if (eve.success != true)
        {
            if (eve.info.ID == currentPuzzleInstance.GetPuzzleID())
            {
                if(eve.success == false)
                {
                    ResetPuzzle();
                }
                    
            }
        }
    }


    //To manage the number of times ResetPuzzle is subscribed to its event, quick fix dont judge pls
    private bool registered = true;
    private void OnResetPuzzle(ResetPuzzleEvent eve)
    {
        if (eve.info.ID == currentPuzzleInstance.GetPuzzleID())
        {
            ResetPuzzle();
        }
    }

    private void ResetPuzzle()
    {
        player = null;

        EventHandler<ResetPuzzleEvent>.UnregisterListener(OnResetPuzzle);
        registered = false;

        symbolPlacer.UnloadSymbols();
        grid.ResetGrid();
    }

    public void RegisterToResetPuzzleEvent()
    {
        if (registered)
            return;

        EventHandler<ResetPuzzleEvent>.RegisterListener(OnResetPuzzle);
        registered = true;
    }
    
    private void PlaceSymbols()
    {
        PlacedSymbols = symbolPlacer.PlaceSymbols(currentPuzzleInstance, symbolPos);
        
    }

    protected void SortTranslations(List<TranslationAndObject> listToSort)
    {
        listToSort.Sort((a, b) => b.translation.Length.CompareTo(a.translation.Length));
        translationsSorted = listToSort;
    }

}

public class TranslationAndObject
{
    public string translation;
    public PuzzleObject pObj;
}



