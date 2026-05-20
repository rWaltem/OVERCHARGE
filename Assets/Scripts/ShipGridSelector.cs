using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipGridSelector : MonoBehaviour
{
    public ShipData shipData;
    public TextMeshProUGUI buttonText;
    public Image buttonBackground;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonText.text = shipData.name;
        buttonBackground.sprite = shipData.icon;
    }
}
