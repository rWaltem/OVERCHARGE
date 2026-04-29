using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class RacePositionTracker : MonoBehaviour
{
    public SplineContainer splineContainer;
    public Transform targetObject;

    [Range(0.001f, 0.2f)]
    public float searchWindow = 0.05f; // how far around last t to search

    private float lastT = 0f;

    public float distance;
    public float length;

    void Update()
    {
        var spline = splineContainer.Spline;
        float3 pos = targetObject.position;

        float bestT = lastT;
        float bestDist = float.MaxValue;

        int steps = 20;

        for (int i = 0; i <= steps; i++)
        {
            float offset = math.lerp(-searchWindow, searchWindow, i / (float)steps);
            float t = Wrap01(lastT + offset);

            float3 splinePoint = spline.EvaluatePosition(t);
            float dist = math.distance(pos, splinePoint);

            if (dist < bestDist)
            {
                bestDist = dist;
                bestT = t;
            }
        }

        // Prevent backwards snapping (important)
        if (!IsForward(lastT, bestT))
        {
            bestT = lastT;
        }

        lastT = bestT;

        length = SplineUtility.CalculateLength(
            spline,
            splineContainer.transform.localToWorldMatrix
        );

        distance = lastT * length;

        Debug.Log(distance);
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

        // handle wrap-around
        if (delta < -0.5f) delta += 1f;
        if (delta > 0.5f) delta -= 1f;

        return delta >= -0.01f; // allow tiny backward tolerance
    }
}