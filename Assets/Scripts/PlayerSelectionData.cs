using UnityEngine;

public class PlayerSelectionData : MonoBehaviour
{
    public static PlayerSelectionData Instance;

    public CharacterData[] selectedCharacters;
    public ShipData[] selectedShips;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            selectedCharacters = new CharacterData[4];
            selectedShips = new ShipData[4];
        }
        else
        {
            Destroy(gameObject);
        }
    }
}