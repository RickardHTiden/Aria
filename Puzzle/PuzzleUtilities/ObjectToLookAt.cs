using UnityEngine;

public class ObjectToLookAt : ObjectToActivate {

    [SerializeField] private Transform end;
    [SerializeField] private LookAtTransitionData transitionData;
    
    public override void Activate(ActivatorEvent eve)
    {
       if (eve.info.ID == puzzleID)
       {
           EventHandler<CameraLookAtEvent>.FireEvent(new CameraLookAtEvent(end, transitionData));
       }
    }
}
