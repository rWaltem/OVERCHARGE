using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System;

[System.Serializable]
public class Racer
{
    public Transform ship;
    public int laps;       // Visual laps (full Möbius loops)
    public float distance; // Total distance for position ranking
}

public class RacePositionTracker : MonoBehaviour
{
    public EventManager eventManager;
    public bool trackPositions = false;
    public Racer[] racers;
    public SplineContainer splineContainer;
    public bool isMobius = false;
    public int totalLaps = 3;

    [Range(0.001f, 0.2f)]
    public float searchWindow = 0.05f;

    public float length; // Physical spline arc length

    public int playerLaps;
    public int playerPos;

    private float[] lastT;
    private int[] splineLapCount;   // Crossings of t=0 on the spline
    private bool trackingInitialized = false;

    void GetRacers()
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Racer");
        var found = new System.Collections.Generic.List<Racer>();

        foreach (GameObject obj in allObjects)
        {
            found.Add(new Racer { ship = obj.transform, laps = 0, distance = 0f });
        }

        racers = found.ToArray();
        lastT = new float[racers.Length];
        splineLapCount = new int[racers.Length];
    }

    public void InitTracking()
    {
        GetRacers();

        var spline = splineContainer.Spline;
        length = SplineUtility.CalculateLength(spline, splineContainer.transform.localToWorldMatrix);
        trackingInitialized = true;
    }

    float GetDistanceAlongSpline(int index)
    {
        var spline = splineContainer.Spline;
        float3 worldPos = racers[index].ship.position;

        float bestT = lastT[index];
        float bestDist = float.MaxValue;

        // Sample candidate t values in a window around last known position
        const int steps = 30;
        for (int i = 0; i <= steps; i++)
        {
            float offset = Mathf.Lerp(-searchWindow, searchWindow, i / (float)steps);
            float t = Wrap01(lastT[index] + offset);

            float3 localPoint = spline.EvaluatePosition(t);
            float3 worldPoint = splineContainer.transform.TransformPoint(localPoint);

            float dist = math.distance(worldPos, worldPoint);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestT = t;
            }
        }

        // Detect forward crossing of the t=0 seam
        float delta = bestT - lastT[index];
        if (delta < -0.5f)       // Wrapped forward over t=1→0
            splineLapCount[index]++;
        else if (delta > 0.5f)   // Wrapped backward — clamp, don't penalise
            bestT = lastT[index];

        lastT[index] = bestT;

        // Total progress in spline-laps (each = one full t=0..1 traversal)
        float totalSplineLaps = splineLapCount[index] + bestT;

        // On a Möbius strip the spline wraps around twice per visual loop,
        // so divide by 2 to get true lap count.
        if (isMobius)
        {
            racers[index].laps = Mathf.FloorToInt(totalSplineLaps / 2f);
            return totalSplineLaps * 0.5f * length;
        }
        else
        {
            racers[index].laps = splineLapCount[index];
            return totalSplineLaps * length;
        }
    }

    void Update()
    {
        if (!trackingInitialized || !trackPositions) return;

        for (int i = 0; i < racers.Length; i++)
            racers[i].distance = GetDistanceAlongSpline(i);

        // Rank by total distance travelled (highest = furthest ahead)
        Array.Sort(racers, (a, b) => b.distance.CompareTo(a.distance));

        // Set player laps and position after sort
        for (int i = 0; i < racers.Length; i++)
        {
            if (racers[i].ship.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerLaps = racers[i].laps + 1;
                playerPos = i + 1; // 1-based position

                if (playerLaps >= totalLaps + 1)
                {
                    eventManager.raceEnded = true;
                }

                break;
            }
        }
    }

    static float Wrap01(float t)
    {
        if (t < 0f) return t + 1f;
        if (t > 1f) return t - 1f;
        return t;
    }
}