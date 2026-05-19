using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("References")]
    public TrackData trackData;
    public RacerSpawner racerSpawner;
    public RacePositionTracker RPT;

    [Header("Time Triggers")]
    public bool raceStarted;
    public bool raceEnded;

    void Start()
    {
        // start corutine for cutscene stuff and loading

        // spawn racers
        racerSpawner.SpawnRacers();

        // initalize tracking logic
        RPT.InitTracking();
        RPT.totalLaps = trackData.laps;

        //while (!raceStarted); // wait until race started == true
    
        // get start signal and
        // set to true after start cutscene and after start of race
        RPT.trackPositions = true;
    }
}
