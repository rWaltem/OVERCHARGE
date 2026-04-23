using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;

    [Header("Public Stats")]
    // 0 - 10 possible
    public int recoverySpeed; // skill of the character

    [Header("Hidden Stats")]
    public string specialty = "n/a";

    [Header("Model")]
    public GameObject characterModelPrefab;
    public Sprite icon;
}
