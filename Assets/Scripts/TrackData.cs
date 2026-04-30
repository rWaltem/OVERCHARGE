using UnityEngine;

[CreateAssetMenu(fileName = "NewTrack", menuName = "Track/Track Data")]
public class TrackData : ScriptableObject
{
    public string trackName;

    [Header("Public Stats")]
    public int laps;
    public float length;

    [Header("Hidden Stats")]
    public string type = "n/a";

    [Header("Track Scene")]
    public string sceneName;
}
