using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public ShipManager playerShipManager;
    public Slider chargeSlider;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI racePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chargeSlider.maxValue = playerShipManager.maxCharge;

    }

    // Update is called once per frame
    void Update()
    {
        // charge value
        chargeSlider.value = playerShipManager.currentCharge;

        // speed value
        speedText.text = $"MPH: {math.round(playerShipManager.currentSpeed)}";
    }
}
