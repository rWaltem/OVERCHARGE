using System.Reflection.Emit;
using Unity.Mathematics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    [Header("Modifiers")]
    public float maxChargeMod = 100;
    public float rechargeRateMod = 1.25f;
    public float thrustMod = 150;

    [Header("Selection")]
    public CharacterData currentCharacter;
    public ShipData currentShip;
    public TrackData currentTrack;

    [Header("Character Stats")]
    public float recoverySpeed;
    
    [Header("Ship Status")]
    public float speed;
    public float accel;
    public float weight;
    public float handling;
    public float maxCharge;
    public float rechargeRate;
    
    [Header("Bonuses")]
    public bool matchingBoost = false;
    public bool specialtyBoost = false;

    [Header("Models")]
    public GameObject characterModelPrefab;
    public GameObject shipModelPrefab;
    public Vector3 shipModelScaleFactor;

    public Vector3 shipModelTargetLocalPosition;
    public Quaternion shipModelTargetLocalRotation;

    [Header("")]
    public float currentCharge;
    public float boostAmount;
    public bool isGrounded;

    [Header("Inputs")]
    public float throttleInput;
    public float brakeInput;
    public float steeringInput;
    public bool boostInput;


    /* PRIVATE VARIABLES */
    private GameObject shipModel;
    private Rigidbody rb;
    private LayerMask trackLayerMask;

    // PID height
    private float shipHeight = 2.5f;
    private float Kp = 500f;
    private float Ki = 0f;
    private float kD = 30f;
    private float integral;
    private float lastError;

    // is set by other scripts to control ship functions
    public void SetInput(float throttle, float brake, float steering, bool boost)
    {
        throttleInput = throttle;
        brakeInput = brake;
        steeringInput = steering;
        boostInput = boost;
    }

    // reads character, ship, and track classes for ship stats (should be from 0 - 10)
    // adds offsets to real usable values
    void GetSelection()
    {
        // Character
        recoverySpeed = currentCharacter.recoverySpeed;

        // Ship
        speed        = currentShip.speed;
        accel        = currentShip.accel;
        weight       = currentShip.weight;
        handling     = currentShip.handling;
        maxCharge    = currentShip.maxCharge * maxChargeMod;
        rechargeRate = currentShip.rechargeRate * rechargeRateMod;

        // models
        characterModelPrefab = currentCharacter.characterModelPrefab;
        shipModelPrefab      = currentShip.shipModelPrefab;

        shipModelScaleFactor = currentShip.shipModelScaleFactor;

        // matching bonus
        if (currentCharacter.specialty == currentShip.specialty) matchingBoost = true;

        // specialty bonus
        if (currentShip.specialty == currentTrack.type) specialtyBoost = true;
    }

    /* Awake is called when the script instance is being loaded */
    void Awake()
    {
        GetSelection();
        Debug.Log("Stats read and set from player selection");

        rb = GetComponent<Rigidbody>();

        trackLayerMask = LayerMask.NameToLayer("Drivable");
    }

    /* Start is called just before any of the Update methods is called the first time */
    void Start()
    {
        // add model to game
        shipModel = Instantiate(shipModelPrefab, transform);
        shipModel.transform.localScale = shipModelScaleFactor;

        Debug.Log("Instantiated ship model");
    }

    /* Controls charge stuff */
    void UpdateCharge()
    {
        //Debug.Log("On boost pad");

        // charge ship
        if (currentCharge < maxCharge) {
            currentCharge += rechargeRate;

            // lock to max charge
            if (currentCharge > maxCharge) currentCharge = maxCharge;
        }
    }

    float Boost()
    {
        // for now, for as long as the player has the boost button pressed, they will get a speed boost
        // but eventually it will be a timed boosted

        // boost will be derived from the speed stat
        // bigger the boost, more charge it uses

        // check if has charge to boost

        // FIXME: NOT EFFICENT HERE, DO THIS ON START/AWAKE 
        // derive boost % from speed stat
        float boostPercent = speed / 10;

        float boostCost = boostPercent * 10; // aka 2/10 speed will be 2 charge per frame

        // FIXME: set boost on a timer instead of button press
        // if there is not enough charge, don't boost --might remove in favor of letting the player explode themselves
        if (currentCharge < boostCost) return 1;

        // decrease charge amount
        currentCharge -= boostCost;

        return 1 + boostPercent; // as a percent
    }

    void AddThrust()
    {
        float thrust;

        if (throttleInput > 0)
        {
            thrust = accel;
        } else if (brakeInput > 0)
        {
            thrust = -accel;
        } else
        {
            thrust = 0;
        }

        float boostThrust = 1;
        if (boostInput) boostThrust = Boost();

        thrust *= thrustMod * boostThrust;
        Debug.Log(thrust);

        rb.AddForce(transform.forward * thrust);
    }

    /* This function is called every fixed framerate frame
       ALL PHYSICS EVENTS FOR SHIP GOES HERE */
    void FixedUpdate()
    {
        rb.linearDamping = 2;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 6))
        {            
            // rotate ship to follow track normal
            Quaternion targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            
            Quaternion newRotation = Quaternion.Lerp(rb.rotation, targetRot, 0.15f);
            rb.MoveRotation(newRotation);

            // ship height PID control
            float error = shipHeight - hit.distance;
            integral += error * Time.fixedDeltaTime; // i term
            float derivative = (error - lastError) / Time.fixedDeltaTime; // d term
            lastError = error;
            float correctingForce = Kp * error + Ki * integral + kD * derivative; // PID output

            //apply pid force
            Vector3 liftDirection = hit.normal;
            rb.AddForce(liftDirection * correctingForce, ForceMode.Acceleration);

            // thrusting logic
            AddThrust();

            // check for boost pad
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Charge Pad")){
                UpdateCharge();
            }
            
        } else
        {
            Debug.DrawRay(transform.position, -transform.up, Color.red);
            rb.linearDamping = 0;
            rb.AddForce(Vector3.down * 78);
        }

        // rotate ship with steering input
        Quaternion steerRotation = Quaternion.Euler(0f, steeringInput * (handling * 20) * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * steerRotation);
    }

    /* Update is called every frame */
    void Update()
    {
        shipModel.transform.localPosition = shipModelTargetLocalPosition;
        shipModel.transform.localRotation = shipModelTargetLocalRotation;
    }
}
