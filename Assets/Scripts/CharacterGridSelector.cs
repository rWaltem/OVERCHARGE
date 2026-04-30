using UnityEngine;

public class CharacterGridSelector : MonoBehaviour
{
    public GameDatabase db;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (CharacterData charData in db.characters)
        {
            Debug.Log(charData.name);
        }
    }
}
