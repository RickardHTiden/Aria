using UnityEngine;

public interface IEvent { }

#region DEBUG_EVENT
public struct DebugInfo
{
    public GameObject obj;
    public int verbosity;
    public string message;
}

public class DebugEvent : IEvent
{
    public DebugInfo Info { get; }
    public DebugEvent(DebugInfo info) => Info = info;
}
#endregion

#region PUZZLE_EVENTS
public class StartPuzzleEvent : IEvent 
{
    public PuzzleInfo info;
    public StartPuzzleEvent(PuzzleInfo info) { this.info = info; }
    //Maybe this should hold puzzle id or position?
}

public class ExitPuzzleEvent : IEvent
{
    public PuzzleInfo info;
    public bool success;
    public ExitPuzzleEvent(PuzzleInfo info, bool state) { this.info = info; this.success = state; }
}


public class ResetPuzzleEvent : IEvent
{
    public PuzzleInfo info;
    public ResetPuzzleEvent(PuzzleInfo info) { this.info = info; }
}

public class ActivatorEvent : IEvent
{
    public PuzzleInfo info;
    public ActivatorEvent(PuzzleInfo info) { this.info = info; }
}


public class PuzzleInfo
{
    public int ID;
    public Puzzle puzzle;
    
    public PuzzleInfo(int id) { ID = id; }
    public PuzzleInfo(int id, Puzzle pp) { ID = id; puzzle = pp; }
}

public readonly struct PlayerStateChangeEvent : IEvent {

    public readonly PlayerState newState;
    public PlayerStateChangeEvent(PlayerState state) => newState = state;
}

public readonly struct CameraLookAtEvent : IEvent {

    public readonly Transform lookAtTarget;
    public readonly LookAtTransitionData transitionData;
    
    public CameraLookAtEvent(Transform lookAtTarget, LookAtTransitionData transitionData) {
        this.lookAtTarget = lookAtTarget;
        this.transitionData = transitionData;
    }
}

public readonly struct CameraLookAndMoveToEvent : IEvent {

    public readonly Transform targetTransform;
    public readonly LookAndMoveTransitionData lookAndMoveTransitionData;

    public CameraLookAndMoveToEvent(Transform targetTransform, LookAndMoveTransitionData lookAndMoveTransitionData) {
        this.targetTransform = targetTransform;
        this.lookAndMoveTransitionData = lookAndMoveTransitionData;
    }
}

public class ClearPuzzleEvent : IEvent
{
    public PuzzleInfo info;
    public ClearPuzzleEvent(PuzzleInfo info) { this.info = info; }
}
public class LoadPuzzleEvent : IEvent
{
    public PuzzleInfo info;
    public LoadPuzzleEvent(PuzzleInfo info) { this.info = info; }
}



public class AwayFromKeyboardEvent : IEvent { }
#endregion

public readonly struct LockInputEvent : IEvent {

    public readonly bool lockInput;

    public LockInputEvent(bool lockInput) => this.lockInput = lockInput;

}
public class SaveEvent : IEvent { }
public class SaveSettingsEvent : IEvent 
{
    public SettingsData settingsData;
    public SaveSettingsEvent(SettingsData data) => settingsData = data;
}

//Hazards
public class UpdateHazardEvent : IEvent
{
    public bool reverse;
    public UpdateHazardEvent (bool isReverse) { reverse = isReverse; }
}
public class ResetHazardEvent : IEvent{}

public class UnLoadSceneEvent : IEvent 
{
    public string sceneToLoad;

    public UnLoadSceneEvent(string scene) { sceneToLoad = scene; }
}

public class LoadSceneEvent : IEvent{}

public class SetUpCameraEvent : IEvent
{
    public Transform followTarget;
    public Transform shoulderPos;

    public SetUpCameraEvent(Transform f, Transform s) { followTarget = f; shoulderPos = s; }
}

public class InGameMenuEvent : IEvent {

    public readonly bool Activate;

    public InGameMenuEvent(bool activate) => Activate = activate;
}

public class TransportationBegunEvent : IEvent {}
public class TransportationEndedEvent : IEvent {}

//Use for making the settings menu to fire a SaveSettingsData
public class RequestSettingsEvent : IEvent {}
public class SceneChangeEvent : IEvent {}
public class SceneLoadedEvent : IEvent {}

