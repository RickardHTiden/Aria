using UnityEngine;

public class ObjectToLookAndMoveTo : ObjectToActivate
{
    [SerializeField] private Transform end;
    [SerializeField] private LookAndMoveTransitionData lookAndMoveTransitionData;
    
    public override void Activate(ActivatorEvent eve)
    {
       
        if (eve.info.ID == puzzleID)
        {
            EventHandler<CameraLookAndMoveToEvent>.FireEvent(
                new CameraLookAndMoveToEvent(end, lookAndMoveTransitionData));
        }
    }
}
