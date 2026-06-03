using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class CPUShipController : MonoBehaviour
{
    [Header("Selection")]
    public CharacterData currentCharacter;
    public ShipData currentShip;
    public GameDatabase gameDatabase;
    public bool randomize = true;

    [Header("AI Tuning")]
    public int refreshRate = 20; // updates per second
    public float lookAhead = 3f;
    public float steeringSensitivity = 4f;

    private ShipManager shipManager;
    private SplineContainer splineContainer;
    private Spline raceLine;

    private float currentT;

    // Refresh timer
    private float refreshTimer;

    // Debug
    private Vector3 debugNearestPoint;
    private Vector3 debugTargetPoint;

    void Awake()
    {
        shipManager = GetComponent<ShipManager>();
    }

    void RandomizeSelection()
    {
        int char_n = gameDatabase.characters.Count();
        int ship_n = gameDatabase.ships.Count();

        int char_r = UnityEngine.Random.Range(0, char_n);
        int ship_r = UnityEngine.Random.Range(0, ship_n);

        shipManager.currentCharacter = gameDatabase.characters[char_r];
        shipManager.currentShip = gameDatabase.ships[ship_r];
    }

    void GetRaceLine()
    {
        GameObject lineObj =
            GameObject.FindGameObjectWithTag("Race Line");

        splineContainer =
            lineObj.GetComponent<SplineContainer>();

        raceLine = splineContainer.Spline;
    }

    void Start()
    {
        if (!randomize)
        {
            shipManager.currentCharacter = currentCharacter;
            shipManager.currentShip = currentShip;
        }
        else
        {
            RandomizeSelection();
        }

        GetRaceLine();
    }

    void UpdateInputs(
        float throttle,
        float brake,
        float steering,
        bool boost
    )
    {
        shipManager.SetInput(
            throttle: throttle,
            brake: brake,
            steering: steering,
            boost: boost
        );
    }

    void FollowSpline()
    {
        if (splineContainer == null)
            return;

        Vector3 shipPos = transform.position;

        // Find nearest point on spline
        SplineUtility.GetNearestPoint(
            raceLine,
            shipPos,
            out float3 nearestPoint,
            out float nearestT
        );

        currentT = nearestT;

        debugNearestPoint = nearestPoint;

        // Look ahead
        Vector3 targetPoint = GetLookAheadPoint(lookAhead);

        // Convert target to local space
        Vector3 localTarget = transform.InverseTransformPoint(targetPoint);

        // Steering
        float steering =
            Mathf.Clamp(
                (localTarget.x / localTarget.magnitude)
                * steeringSensitivity,
                -1f,
                1f
            );

        // Speed control
        float throttle = 1f;
        float brake = 0f;

        if (Mathf.Abs(steering) > 0.5f)
            throttle = 0.6f;

        if (Mathf.Abs(steering) > 0.8f)
            throttle = 0.3f;

        UpdateInputs(
            throttle,
            brake,
            steering,
            false
        );
    }

    Vector3 GetLookAheadPoint(float distanceAhead)
    {
        if (splineContainer == null)
            return transform.position;

        // Get nearest spline position
        SplineUtility.GetNearestPoint(
            raceLine,
            transform.position,
            out float3 nearestPoint,
            out float nearestT
        );

        currentT = nearestT;

        // Convert distance into normalized spline movement
        float targetT = currentT + (distanceAhead * 0.01f);

        // Loop spline
        if (targetT > 1f)
            targetT -= 1f;

        Vector3 targetPoint =
            splineContainer.EvaluatePosition(targetT);

        // Debug
        debugNearestPoint = nearestPoint;
        debugTargetPoint = targetPoint;

        return targetPoint;
    }

    void Update()
    {
        if (refreshRate <= 0)
        {
            FollowSpline();
            return;
        }

        refreshTimer += Time.deltaTime;

        float refreshInterval = 1f / refreshRate;

        if (refreshTimer >= refreshInterval)
        {
            refreshTimer = 0f;
            FollowSpline();
        }
    }

    void OnDrawGizmos()
    {
        // Green = closest spline point
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(debugNearestPoint, 0.5f);

        // Red = steering target
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(debugTargetPoint, 0.7f);

        // Yellow line = where AI is steering
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, debugTargetPoint);
    }
}