using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGridSelector : MonoBehaviour
{
    public PlayerSelectionObject playerSelectionObject;
    public CharacterData charData;
    public TextMeshProUGUI buttonText;
    public Image buttonBackground;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonText.text = charData.name;
        buttonBackground.sprite = charData.icon;

        playerSelectionObject.character = null;
    }

    public void Select()
    {
        playerSelectionObject.character = charData;
    }
}
