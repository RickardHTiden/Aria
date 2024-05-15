using UnityEngine;

public class PuzzleStarter : MonoBehaviour
{
    private Puzzle puzzle;
    private int puzzleID;

    public bool active;

    private void Start()
    {
        puzzle = GetComponentInParent<Puzzle>();
        puzzleID = puzzle.GetPuzzleID();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (active == false)
        {
            puzzle.GetComponent<SphereCollider>().enabled = true;
            EventHandler<StartPuzzleEvent>.FireEvent(new StartPuzzleEvent(new PuzzleInfo(puzzle.GetPuzzleID(), GetComponentInParent<Puzzle>())));
            puzzle.SetPlayer(other.transform);
            active = true;
        }
    }


    public void ResetStarter()
    {
        //Debug.Log("RESET STARTER");
            active = false;
    }


}