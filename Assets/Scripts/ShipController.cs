using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
//using UnityEditor.ShaderGraph.Internal;
using Unity.Mathematics;

public class ShipController : MonoBehaviour
{
    [Header("Ship Charge")]
    public int maxCharge = 100;
    public int currentCharge = 100;
    public bool onChargePad = false;
    public bool isBoosting = false;
    public int chargeRate = 10;
    public float boostForce = 20000f;

    [Header("Ship Settings")]
    public LayerMask groundLayer;
    public float forwardAccel = 12f;
    public float reverseAccelPercent = 0.6f;
    public float turnStrength;
    public float shipNormalRotSmoothing;
    public float groundingForceMultiplier = 100f;
    public float linearDamping = 1.75f;
    public float boostDamping = 1.25f;


    // ground
    public float localGravityRayLength = 10f;
    public float shipHeight = 2f;

    // private variables
    private GetStats playerStats;
    private Rigidbody rb;
    private Vector3 gravityVector;
    private float speedInput;
    private float turnInput;

    // Input
    private InputSystem_Actions playerControls;
    private InputAction throttleControl;
    private InputAction brakeControl;
    private InputAction steeringControl;
    private InputAction boostControl;

    void Awake()
    {
        playerStats = GetComponent<GetStats>();
        playerControls = new InputSystem_Actions();
    }

    void OnEnable()
    {
        throttleControl = playerControls.Player.Throttle;
        throttleControl.Enable();

        brakeControl = playerControls.Player.Brake;
        brakeControl.Enable();

        steeringControl = playerControls.Player.Steering;
        steeringControl.Enable();

        boostControl = playerControls.Player.Boost;
        boostControl.Enable();
    }

    void OnDisable()
    {
        throttleControl.Disable();
        brakeControl.Disable();
        steeringControl.Disable();
        boostControl.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentCharge = maxCharge;
    }

    // Update is called once per frame
    void Update()
    {
        float throttleVal = throttleControl.ReadValue<float>();
        float brakeVal = brakeControl.ReadValue<float>();
        float steeringInput = steeringControl.ReadValue<float>();
        float boostInput = boostControl.ReadValue<float>();

        if (boostInput > 0 && currentCharge > 0)
        {
            isBoosting = true;
        } else
        {
            isBoosting = false;
        }

        turnInput = steeringInput;
    }

    void FixedUpdate()
    {
        RaycastHit hit;

        // raycast for ground and gravity
        if (Physics.Raycast(rb.transform.position, -transform.up, out hit, localGravityRayLength, groundLayer))
        {
            rb.linearDamping = linearDamping;

            // find gravity vector
            gravityVector = -hit.normal;
            //Debug.DrawRay(transform.position, gravityVector, Color.green);

            // rotate model to follow track normal
            Quaternion targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, shipNormalRotSmoothing);

            // turn ship
            transform.Rotate(0f, turnInput * turnStrength * Time.deltaTime, 0f, Space.Self);

            // grounding force
            // add force down in a neg exponetal curve, lots when far away and little when close
            float groundingForce = math.pow(hit.distance - shipHeight, 3) * groundingForceMultiplier; // expo needs to be odd
            rb.AddForce(gravityVector * groundingForce);
            //Debug.DrawRay(rb.transform.position, gravityVector * groundingForce);

            // forward force
            if (isBoosting)
            {
                speedInput = boostForce;
                rb.linearDamping = boostDamping;
                currentCharge--;
            } else
            {
                speedInput = forwardAccel * throttleControl.ReadValue<float>();
            }

            rb.AddForce(transform.forward * speedInput);
            
        } else
        {
            gravityVector = Vector3.down;
            rb.linearDamping = 0f;
        }
    }
}
