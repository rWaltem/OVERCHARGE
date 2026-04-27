using System.Reflection.Emit;
using Unity.Mathematics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    [Header("Modifiers")]
    public GlobalShipModifiers globalMods;

    [Header("Selection")]
    public CharacterData currentCharacter;
    public ShipData currentShip;
    public TrackData currentTrack;

    [Header("Character Stats")]
    public float recoverySpeed;
    
    [Header("Ship Stats")]
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
    public LayerMask trackLayerMask;
    public LayerMask chargePadLayerMask;
    public float boostDuration = 2.5f; // how long boost last
    public float boostCost = 50f; // cost per activation
    public bool isJammed = false;
    

    [Header("Inputs")]
    public float throttleInput;
    public float brakeInput;
    public float steeringInput;
    public bool boostInput;


    /* PRIVATE VARIABLES */
    private GameObject shipModel;
    private Rigidbody rb;
    private bool isBoosting = false;
    private float boostTimer = 0f;
    private float boostSpeed;
    private bool lastBoostInput;
    private float recoveryTime;
    public float currentSpeed = 0f;
    private float currentSteer = 0f;
    private bool isCharging = false;
    private int chargeContacts;

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
        if (isJammed)
        {
            throttleInput = 0;
            brakeInput = 0;
            steeringInput = 0;
            boostInput = false;
            isBoosting = false;

            return;
        }

        throttleInput = throttle;
        brakeInput = brake;
        steeringInput = steering;
        boostInput = boost;
    }

    // reads character, ship, and track classes for ship stats (should be from 0 - 10)
    // adds offsets to real usable values
    void GetSelection()
    {
        // boost
        boostSpeed = currentShip.boostSpeed;
        boostDuration = globalMods.boostDuration;
        boostCost = globalMods.boostCost;
        
        // Character
        recoverySpeed = currentCharacter.recoverySpeed * globalMods.recoverySpeedMod;
        

        // Ship
        speed        = currentShip.speed        * globalMods.speedMod;
        accel        = currentShip.accel        * globalMods.accelMod;
        weight       = currentShip.weight       * globalMods.weightMod;
        handling     = currentShip.handling     * globalMods.handlingMod;
        maxCharge    = currentShip.maxCharge    * globalMods.maxChargeMod;
        rechargeRate = currentShip.rechargeRate * globalMods.rechargeRateMod;

        // matching bonus
        if (currentCharacter.specialty == currentShip.specialty)
        {
            Debug.Log("Has proficiency boost");
            matchingBoost = true;
            recoverySpeed *= globalMods.matchingBoostMod;
        }

        // specialty bonus
        if (currentShip.specialty == currentTrack.type)
        {
            Debug.Log("Has specialty boost");
            specialtyBoost = true;
            speed *= globalMods.specialtyBoostMod;
        }

        // models
        characterModelPrefab = currentCharacter.characterModelPrefab;
        shipModelPrefab      = currentShip.shipModelPrefab;

        shipModelScaleFactor = currentShip.shipModelScaleFactor;

        Debug.Log("Stats read and set from player selection");
    }

    /* Awake is called when the script instance is being loaded */
    void Awake()
    {
        GetSelection();

        rb = GetComponent<Rigidbody>();
    }

    /* Start is called just before any of the Update methods is called the first time */
    void Start()
    {
        // add model to game
        shipModel = Instantiate(shipModelPrefab, transform);
        shipModel.transform.localScale = shipModelScaleFactor;
        shipModel.transform.localPosition = shipModelTargetLocalPosition;
        shipModel.transform.localRotation = shipModelTargetLocalRotation;

        Debug.Log("Instantiated ship model");

        currentCharge = maxCharge / 2;
        Debug.Log("Set start charge");
    }

    /* Controls charge stuff */
    void UpdateCharge()
    {
        //Debug.Log("On boost pad");

        if (!isCharging) return;

        // charge ship
        if (currentCharge < maxCharge) {
            currentCharge += rechargeRate * Time.fixedDeltaTime;

            // lock to max charge
            if (currentCharge > maxCharge) currentCharge = maxCharge;
        }
    }

    void TryBoost()
    {
        if (isBoosting) return;

        currentCharge -= boostCost;

        isBoosting = true;
        boostTimer = boostDuration;
    }

    void UpdateSpeed()
    {
        float input = 0f;

        if (isBoosting || throttleInput > 0f)
        {
            input = 1f;
        }
        else if (brakeInput > 0f)
        {
            input = -1f;
        }
        
        // boost multiplier
        float boostMultiplier = 1;
        if (isBoosting) boostMultiplier = boostSpeed;

        float targetSpeed = input * (speed * boostMultiplier);

        // distance to target (used for S-curve shaping)
        float speedDiff = targetSpeed - currentSpeed;
        float absDiff = Mathf.Abs(speedDiff);

        // normalize progress toward target (0..1)
        float normalized = Mathf.Clamp01(absDiff / speed);

        // S-curve easing (slow start, fast middle, slow end)
        float sCurve = normalized * normalized * (3f - 2f * normalized);

        float accelThisFrame = accel * boostMultiplier * sCurve * Time.fixedDeltaTime;

        // move current speed toward target speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelThisFrame);
        //Debug.Log($"Current Speed: {currentSpeed}");

        Vector3 velocity = transform.forward * currentSpeed;

        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    void UpdateSteering()
    {
        // input is -1 to 1
        float targetSteer = steeringInput;

        // how fast steering responds based on handling
        float steerSpeed = handling;

        // smooth steering response
        currentSteer = Mathf.MoveTowards(currentSteer, targetSteer, steerSpeed * Time.fixedDeltaTime);

        // turn rate scales with handling and current speed
        float turnStrength = handling * (1f + Mathf.Abs(currentSpeed) / speed);

        float turnAmount = currentSteer * turnStrength * Time.fixedDeltaTime;

        Quaternion steerRotation = Quaternion.Euler(0f, turnAmount, 0f);

        rb.MoveRotation(rb.rotation * steerRotation);
    }

    /* This function is called every fixed framerate frame
       ALL PHYSICS EVENTS FOR SHIP GOES HERE */
    void FixedUpdate()
    {
        rb.linearDamping = 2;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 6))
        {            
            isGrounded = true;

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

            UpdateSpeed();
            UpdateSteering();
            UpdateCharge();
        } else
        {
            isGrounded = false;

            Debug.DrawRay(transform.position, -transform.up, Color.red);
            rb.linearDamping = 0;
            rb.AddForce(Vector3.down * 78);
        }
    }

    void JamDelay()
    {
        recoveryTime -= Time.deltaTime;

        if (recoveryTime <= 0f)
        {
            isJammed = false;
            currentCharge = 1f;
        }

        //Debug.Log($"Recovery Time: {recoveryTime}");
    }

    // used for updating ship model, ie for animation
    void UpdateShipTransform()
    {
        return;
    }

    /* Update is called every frame */
    void Update()
    {
        // detect button press
        if (boostInput && !lastBoostInput)
        {
            TryBoost();
        }
        lastBoostInput = boostInput;

        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;

            if (boostTimer <= 0f)
            {
                isBoosting = false;
            }
        }

        if (currentCharge <= 0 && !isJammed)
        {
            isJammed = true;
            recoveryTime = recoverySpeed;
        }


        if (chargeContacts > 0)
        {
            isCharging = true;
        } else
        {
            isCharging = false;
        }

        if (isJammed) JamDelay();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Charge Pad"))
        {
            chargeContacts++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Charge Pad"))
        {
            chargeContacts--;
            if (chargeContacts < 0) chargeContacts = 0; // safety
        }
    }
}