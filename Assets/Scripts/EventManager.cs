using UnityEngine;

public class EventManager : MonoBehaviour
{
    public TrackData trackData;
    public RacerSpawner racerSpawner;
    public RacePositionTracker RPT;

    void Start()
    {
        // spawn racers
        racerSpawner.SpawnRacers();

        // initalize tracking logic
        RPT.InitTracking();

        // do cutscene stuff here

        // get start signal and
        // set to true after start cutscene and after start of race
        RPT.trackPositions = true;
    }
}
