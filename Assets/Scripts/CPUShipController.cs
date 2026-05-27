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
    public float lookAhead = 15f;
    public float steeringSensitivity = 2f;
    public float waypointReach = 2f;

    private ShipManager shipManager;
    private SplineContainer splineContainer;
    private Spline raceLine;

    private float currentT;

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
        GameObject lineObj = GameObject.FindGameObjectWithTag("Race Line");

        splineContainer = lineObj.GetComponent<SplineContainer>();
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

    void UpdateInputs(float throttle, float brake, float steering, bool boost)
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
        if (raceLine == null)
            return;

        Vector3 shipPos = transform.position;

        // Find closest point on spline
        SplineUtility.GetNearestPoint(
            raceLine,
            shipPos,
            out float3 nearestPoint,
            out currentT
        );

        // Look ahead on spline
        float targetT = currentT + (lookAhead / raceLine.Count);

        if (targetT > 1f)
            targetT -= 1f;

        Vector3 targetPoint = splineContainer.EvaluatePosition(targetT);

        // Direction to target
        Vector3 localTarget =
            transform.InverseTransformPoint(targetPoint);

        // Steering
        float steering =
            Mathf.Clamp(localTarget.x * steeringSensitivity, -1f, 1f);

        // Slow down slightly for hard turns
        float throttle = 1f;
        float brake = 0f;

        if (Mathf.Abs(steering) > 0.7f)
        {
            throttle = 0.5f;
        }

        UpdateInputs(
            throttle,
            brake,
            steering,
            false
        );
    }

    void Update()
    {
        FollowSpline();
    }
}