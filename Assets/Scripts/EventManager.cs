using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public enum GameState
    {
        Loading,
        Intro,
        Runtime,
        Summary,
        Failed
    }

    [Header("Game State")]
    public GameState currentGameState = GameState.Loading;


    [Header("References")]
    public TrackData trackData;
    public RacerSpawner racerSpawner;
    public RacePositionTracker RPT;

    void Start()
    {
        // spawn racers
        racerSpawner.SpawnRacers();
    }

    void Update()
    {   
        // set to intro if done loading
        if (currentGameState == GameState.Loading & racerSpawner.doneSpawning == true)
            currentGameState = GameState.Intro;
            
            // initalize tracking logic
            RPT.InitTracking();
            RPT.totalLaps = trackData.laps;

        switch (currentGameState)
        {
            case GameState.Loading:
                break;

            case GameState.Intro:
                break;

            case GameState.Runtime:
                RPT.trackPositions = true;
                break;

            case GameState.Summary:
                break;
            
            case GameState.Failed:
                break;
        }
    }
}
