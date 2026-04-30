using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "Game/Game Database")]
public class GameDatabase : ScriptableObject
{
    [Header("Characters")]
    public List<CharacterData> characters = new List<CharacterData>();

    [Header("Ships")]
    public List<ShipData> ships = new List<ShipData>();

    [Header("Tracks")]
    public List<TrackData> tracks = new List<TrackData>();
}