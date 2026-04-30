using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System;

[System.Serializable]
public class racer
{
    public Transform ship;
    public float laps;
    public float distance; // <-- store current distance
}

public class RacePositionTracker : MonoBehaviour
{
    public racer[] racers;
    public SplineContainer splineContainer;
    public bool isMobius = false;

    [Range(0.001f, 0.2f)]
    public float searchWindow = 0.05f;

    public float length;

    private float[] lastT;
    private int[] lapCount;

    void Awake()
    {
        var spline = splineContainer.Spline;

        length = SplineUtility.CalculateLength(
            spline,
            splineContainer.transform.localToWorldMatrix
        );

        lastT = new float[racers.Length];
        lapCount = new int[racers.Length];
    }

    float GetDistanceAlongSpline(int index)
    {
        var spline = splineContainer.Spline;

        float3 worldPos = racers[index].ship.position;

        float bestT = lastT[index];
        float bestDist = float.MaxValue;

        int steps = 20;

        for (int i = 0; i <= steps; i++)
        {
            float offset = math.lerp(-searchWindow, searchWindow, i / (float)steps);
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

        // Lap detection
        if (IsForward(lastT[index], bestT))
        {
            if (bestT < lastT[index] - 0.5f)
            {
                lapCount[index]++;
            }
        }
        else
        {
            bestT = lastT[index];
        }

        lastT[index] = bestT;

        float totalT = lapCount[index] + bestT;
        float effectiveT = isMobius ? totalT * 0.5f : totalT;

        return effectiveT * length;
    }

    void Update()
    {
        // Update distances
        for (int i = 0; i < racers.Length; i++)
        {
            racers[i].distance = GetDistanceAlongSpline(i);
        }

        // Sort racers by distance (highest first)
        Array.Sort(racers, (a, b) => b.distance.CompareTo(a.distance));

        // Debug output (now in race order)
        //for (int i = 0; i < racers.Length; i++)
        //{
        //    Debug.Log($"Place {i + 1}: {racers[i].ship.name} ({racers[i].distance / length})");
        //}
    }

    float Wrap01(float t)
    {
        if (t < 0f) return t + 1f;
        if (t > 1f) return t - 1f;
        return t;
    }

    bool IsForward(float from, float to)
    {
        float delta = to - from;

        if (delta < -0.5f) delta += 1f;
        if (delta > 0.5f) delta -= 1f;

        return delta >= -0.01f;
    }
}