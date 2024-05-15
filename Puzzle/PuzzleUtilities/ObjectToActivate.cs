using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectToActivate : MonoBehaviour
{
    [SerializeField] protected int puzzleID;

    private void OnEnable()
    {
        EventHandler<ActivatorEvent>.RegisterListener(Activate);
    }

    private void OnDisable()
    {
        EventHandler<ActivatorEvent>.UnregisterListener(Activate);
    }

    abstract public void Activate(ActivatorEvent eve);
}
