using System.Reflection.Emit;
using Unity.Mathematics;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    [Header("Modifiers")]
    public float maxChargeMod = 100;
    public float thrustMod = 200;

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
        rechargeRate = currentShip.rechargeRate;

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
        //Debug.Log("Got rigidbody");

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
        Debug.Log("On boost pad");

        // charge ship
        if (currentCharge < maxCharge) {
            currentCharge += rechargeRate;

            // lock to max charge
            if (currentCharge > maxCharge) currentCharge = maxCharge;
        }

        // update shit here mate
        // like raycast
        // and current charge
        // and recharge
        // and boost
    }

    /* Update is called every frame */
    void Update()
    {
        shipModel.transform.localPosition = shipModelTargetLocalPosition;
        shipModel.transform.localRotation = shipModelTargetLocalRotation;
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

        thrust *= thrustMod;

        // TODO: if player is boosting, give them extra thrust

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
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.15f);

            // ship height PID control
            float error = shipHeight - hit.distance;
            integral += error * Time.fixedDeltaTime; // integral
            float derivative = (error - lastError) / Time.fixedDeltaTime; // derivative
            lastError = error;
            float correctingForce = Kp * error + Ki * integral + kD * derivative; //PID output

            //apply pid force
            Vector3 liftDirection = hit.normal;
            rb.AddForce(liftDirection * correctingForce, ForceMode.Force);

            // thrusting logic
            AddThrust();

            // check for boost pad
            if (hit.transform.gameObject.layer == 6){ // change 6 to "Boost Pad" at some point
                UpdateCharge();
            }
            
        } else
        {
            Debug.DrawRay(transform.position, -transform.up, Color.red);
            rb.linearDamping = 0;
            rb.AddForce(Vector3.down * 78);
        }

        // rotate ship with steering input
        transform.Rotate(0f, steeringInput * (handling * 20) * Time.deltaTime, 0f, Space.Self);

    }
}
