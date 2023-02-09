using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Game Events", order = 51)]
public class GameEvents : ScriptableObject
{

    [Header("Game Events")] 
    public GameEvent NewGameEvent;
    public GameEvent GameOverEvent;
    //public GameEventVector2 SpawnEnemyEvent;
    
    [Header("UI Events")]
    public GameEvent UpdateUIEvent;
    public GameEventStringFloat MessageEvent;
    public GameEventAudio AudioEvent;
    
    [Header("Screen Change Events")]
    public GameEventVector2 MovePlayerEvent;
    public GameEventVector2 MoveCameraEvent;
    public GameEventVector2 StopCameraEvent;
    
}
