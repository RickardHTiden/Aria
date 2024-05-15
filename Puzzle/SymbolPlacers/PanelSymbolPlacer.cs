using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSymbolPlacer : SymbolPlacer
{
    protected override void UnevenPlaceSymbols()
    {
        Quaternion emptyQ = new Quaternion(0, 0, 0, 0);
        int mid = instantiatedSymbols.Count / 2;
        instantiatedSymbols[mid].transform.localPosition = Vector3.zero;
        instantiatedSymbols[mid].transform.localRotation = emptyQ;

        for (int i = 1; i <= mid; i++)
        {
            Vector3 tempPos = Vector3.zero;
            tempPos -= i * (symbolOffset * Vector3.right);
            instantiatedSymbols[mid - i].transform.localPosition = tempPos;
            instantiatedSymbols[mid - i].transform.rotation = symbolPos.rotation;
            instantiatedSymbols[mid - i].transform.localPosition =
                new Vector3(instantiatedSymbols[mid - i].transform.localPosition.x, 0, 0);

            tempPos = Vector3.zero;
            tempPos += i * (symbolOffset * Vector3.right);
            instantiatedSymbols[mid + i].transform.localPosition = tempPos;
            instantiatedSymbols[mid + i].transform.rotation = symbolPos.rotation;
            instantiatedSymbols[mid + i].transform.localPosition =
                new Vector3(instantiatedSymbols[mid + i].transform.localPosition.x, 0, 0);
        }
    }

    protected override void EvenPlaceSymbols()
    {
        int midRight = instantiatedSymbols.Count / 2;
        int midLeft = midRight - 1;

        Vector3 midLeftPos = (Vector3.left * (symbolOffset / 2));
        Vector3 midRightPos = (Vector3.right * (symbolOffset / 2));

        instantiatedSymbols[midLeft].transform.localPosition = midLeftPos;
        instantiatedSymbols[midLeft].transform.localRotation = new Quaternion(0, 0, 0, 0);

        instantiatedSymbols[midRight].transform.localPosition = midRightPos;
        instantiatedSymbols[midRight].transform.rotation = new Quaternion(0, 0, 0, 0);

        for (int i = 1; i <= midLeft; i++)
        {
            Vector3 tempPos = midLeftPos;
            tempPos -= i * (symbolOffset * Vector3.right);
            instantiatedSymbols[midLeft - i].transform.localPosition = tempPos;
            instantiatedSymbols[midLeft - i].transform.rotation = symbolPos.rotation;

            tempPos = midRightPos;
            tempPos += i * (symbolOffset * Vector3.right);
            instantiatedSymbols[midRight + i].transform.localPosition = tempPos;
            instantiatedSymbols[midRight + i].transform.rotation = symbolPos.rotation;
        }
    }
}
