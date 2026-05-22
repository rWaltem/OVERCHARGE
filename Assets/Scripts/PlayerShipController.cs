using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShipController : MonoBehaviour
{
    public PlayerSelectionObject playerSelectionObject;

    [Header("Player Inputs")]
    public float throttle;
    public float brake;
    public float steering;
    public bool boost;

    private ShipManager shipManager;

    private InputSystem_Actions playerControls;
    private InputAction throttleInput;
    private InputAction brakeInput;
    private InputAction steeringInput;
    private InputAction boostInput;

    void Awake()
    {
        playerControls = new InputSystem_Actions();
        Debug.Log("New player input system");

        shipManager = GetComponent<ShipManager>();
        Debug.Log("Player has shipManager");
    }

    void Start()
    {
        // set character selection
        shipManager.currentCharacter = playerSelectionObject.character;
        shipManager.currentShip      = playerSelectionObject.ship;        
    }

    void OnEnable()
    {
        throttleInput = playerControls.Player.Throttle;
        throttleInput.Enable();

        brakeInput = playerControls.Player.Brake;
        brakeInput.Enable();

        steeringInput = playerControls.Player.Steering;
        steeringInput.Enable();

        boostInput = playerControls.Player.Boost;
        boostInput.Enable();
    }

    void OnDisable()
    {
        throttleInput.Disable();
        brakeInput.Disable();
        steeringInput.Disable();
        boostInput.Disable();
    }

    void Update()
    {

        throttle = throttleInput.ReadValue<float>();
        brake    = brakeInput.ReadValue<float>();
        steering = steeringInput.ReadValue<float>();
        boost    = boostInput.ReadValue<float>() > 0;

        // set ship inputs
        shipManager.SetInput(
            throttle : throttle,
            brake    : brake,
            steering : steering,
            boost    : boost
        );

        
    }
}
