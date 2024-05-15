using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCounter : MonoBehaviour
{
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private float markerOffset;
    private List<GameObject> allMarkers = new List<GameObject>();

    public void GenerateMarkers(int puzzleAmount)
    {
        for(int i = 0; i < puzzleAmount; i++)
        {
            allMarkers.Add(Instantiate(markerPrefab, transform));
        }
        PlaceMarkers();
        allMarkers[0].GetComponent<Animator>().SetTrigger("current");
    }

    private void PlaceMarkers()
    {
        if (allMarkers.Count % 2 == 0)
            EvenPlaceSymbols();
        else
            UnevenPlaceSymbols();
    }

    public void MarkedAsSolved(int indexOfPuzzle)
    {
        allMarkers[indexOfPuzzle].GetComponent<Animator>().SetTrigger("solved");
        if (indexOfPuzzle + 1 < allMarkers.Count)
            allMarkers[indexOfPuzzle + 1].GetComponent<Animator>().SetTrigger("current");
    }

    public void DeleteMarkers()
    {
        for(int i = 0; i < allMarkers.Count; i++)
        {
            Destroy(allMarkers[i]);
        }
        allMarkers.Clear();
    }

    private void UnevenPlaceSymbols()
    {
        Quaternion emptyQ = new Quaternion(0, 0, 0, 0);
        int mid = allMarkers.Count / 2;
        allMarkers[mid].transform.localPosition = Vector3.zero;
        allMarkers[mid].transform.localRotation = emptyQ;

        for (int i = 1; i <= mid; i++)
        {
            Vector3 tempPos = Vector3.zero;
            tempPos -= i * (markerOffset * Vector3.right);
            allMarkers[mid - i].transform.localPosition = tempPos;
            allMarkers[mid - i].transform.rotation = transform.rotation;
            allMarkers[mid - i].transform.localPosition =
                new Vector3(allMarkers[mid - i].transform.localPosition.x, 0, 0);



            tempPos = Vector3.zero;
            tempPos += i * (markerOffset * Vector3.right);
            allMarkers[mid + i].transform.localPosition = tempPos;
            allMarkers[mid + i].transform.rotation = transform.rotation;
            allMarkers[mid + i].transform.localPosition =
                new Vector3(allMarkers[mid + i].transform.localPosition.x, 0, 0);
        }
    }

    private void EvenPlaceSymbols()
    {


        int midRight = allMarkers.Count / 2;
        int midLeft = midRight - 1;

        Vector3 midLeftPos = (Vector3.left * (markerOffset / 2));
        Vector3 midRightPos = (Vector3.right * (markerOffset / 2));

        allMarkers[midLeft].transform.localPosition = midLeftPos;
        allMarkers[midLeft].transform.localRotation = new Quaternion(0, 0, 0, 0);

        allMarkers[midRight].transform.localPosition = midRightPos;
        allMarkers[midRight].transform.rotation = new Quaternion(0, 0, 0, 0);

        for (int i = 1; i <= midLeft; i++)
        {
            Vector3 tempPos = midLeftPos;
            tempPos -= i * (markerOffset * Vector3.right);
            allMarkers[midLeft - i].transform.localPosition = tempPos;
            allMarkers[midLeft - i].transform.rotation = transform.rotation;

            tempPos = midRightPos;
            tempPos += i * (markerOffset * Vector3.right);
            allMarkers[midRight + i].transform.localPosition = tempPos;
            allMarkers[midRight + i].transform.rotation = transform.rotation;
        }
    }
}
