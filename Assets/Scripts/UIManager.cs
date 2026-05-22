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
    public TextMeshProUGUI lapCount;

    private RacePositionTracker rpt;
    void Awake()
    {
        rpt = FindObjectsByType<RacePositionTracker>()[0];
    }

    // Update is called once per frame
    void Update()
    {
        // charge value
        chargeSlider.maxValue = playerShipManager.maxCharge;
        chargeSlider.value = playerShipManager.currentCharge;

        // speed value
        speedText.text = $"MPH: {math.round(playerShipManager.currentSpeed)}";
        racePos.text = $"Pos: {rpt.playerPos}";
        lapCount.text = $"Lap: {rpt.playerLaps}";
    }
}
