using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Groups")]
    public GameObject loadingGroup;
    public GameObject introGroup;
    public GameObject runtimeGroup;
    public GameObject summaryGroup;
    public GameObject failGroup;

    [Header("Runtime UI")]
    public ShipManager playerShipManager;
    public Slider chargeSlider;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI racePos;
    public TextMeshProUGUI lapCount;

    private GameObject gameManager;
    private EventManager eventManager;
    private RacePositionTracker rpt;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("Game Manager");
        eventManager = gameManager.GetComponent<EventManager>();
        rpt = gameManager.GetComponent<RacePositionTracker>();

        loadingGroup.SetActive(false);
        introGroup.SetActive(false);
        runtimeGroup.SetActive(false);
        summaryGroup.SetActive(false);
        failGroup.SetActive(false);

    }

    void Loading()
    {
        if (loadingGroup.activeInHierarchy != true)
        {
            loadingGroup.SetActive(true);
            introGroup.SetActive(false);
            runtimeGroup.SetActive(false);
            summaryGroup.SetActive(false);
            failGroup.SetActive(false);
        }
    }

    void Intro()
    {
        if (introGroup.activeInHierarchy != true) 
        {
            loadingGroup.SetActive(false);
            introGroup.SetActive(true);
            runtimeGroup.SetActive(false);
            summaryGroup.SetActive(false);
            failGroup.SetActive(false);
        }
    }

    void Runtime()
    {
        if (runtimeGroup.activeInHierarchy != true)
        {
            loadingGroup.SetActive(false);
            introGroup.SetActive(false);
            runtimeGroup.SetActive(true);
            summaryGroup.SetActive(false);
            failGroup.SetActive(false);
        }

        chargeSlider.maxValue = playerShipManager.maxCharge;
        chargeSlider.value = playerShipManager.currentCharge;

        speedText.text = $"MPH: {math.round(playerShipManager.currentSpeed)}";
        racePos.text = $"Pos: {rpt.playerPos}";
        lapCount.text = $"Lap: {rpt.playerLaps}";
    }

    void Summary()
    {
        if (summaryGroup.activeInHierarchy != true) 
        {
            loadingGroup.SetActive(false);
            introGroup.SetActive(false);
            runtimeGroup.SetActive(false);
            summaryGroup.SetActive(true);
            failGroup.SetActive(false);

        }
    }

    void Failed()
    {
        if (failGroup.activeInHierarchy != true)
        {
            loadingGroup.SetActive(false);
            introGroup.SetActive(false);
            runtimeGroup.SetActive(false);
            summaryGroup.SetActive(false);
            failGroup.SetActive(true);
        }
    }


    private void Update()
    {
        switch (eventManager.currentGameState)
        {
            case EventManager.GameState.Loading:
                // loading screen UI
                Loading();
                break;

             case EventManager.GameState.Intro:
                // Intro UI
                Intro();
                break;
            
            case EventManager.GameState.Runtime:
                // Runtime UI
                Runtime();
                break;
            
            case EventManager.GameState.Summary:
                // end race summary UI
                Summary();
                break;
            
            case EventManager.GameState.Failed:
                // Failed
                Failed();
                break;
        }
    }
}