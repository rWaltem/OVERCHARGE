using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSelection", menuName = "Selection/Player Selection")]
public class PlayerSelectionObject : ScriptableObject
{
    public CharacterData character;

    public ShipData ship;

    public TrackData track;
}