using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TrackSurfaceGenerator : MonoBehaviour
{
    public SplineContainer splineContainer;

    [Header("Mesh Settings")]
    public int resolution = 50;
    public float width = 1f;

    private Mesh mesh;

    void Awake()
    {
        if (!splineContainer)
        {
            splineContainer = GetComponent<SplineContainer>();

            if (!splineContainer)
            {
                Debug.LogError("No SplineContainer found on this GameObject.");
            }
        }
    }

    public void GenerateMesh()
    {
        if (splineContainer == null) return;

        Spline spline = splineContainer.Spline;
        bool closed = spline.Closed;

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Track Mesh";
        }
        else
        {
            mesh.Clear();
        }

        GetComponent<MeshFilter>().sharedMesh = mesh;

        int pointCount = closed ? resolution : resolution + 1;
        int vertCount = pointCount * 2;

        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];

        int segmentCount = closed ? resolution : resolution;
        int[] triangles = new int[segmentCount * 6];

        // --- Generate vertices ---
        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)resolution;

            Vector3 position = (Vector3)spline.EvaluatePosition(t);
            Vector3 tangent = ((Vector3)spline.EvaluateTangent(t)).normalized;
            Vector3 up = ((Vector3)spline.EvaluateUpVector(t)).normalized;

            Vector3 right = Vector3.Cross(up, tangent).normalized;

            int vi = i * 2;

            vertices[vi] = position - right * width * 0.5f;
            vertices[vi + 1] = position + right * width * 0.5f;

            uvs[vi] = new Vector2(0, t);
            uvs[vi + 1] = new Vector2(1, t);
        }

        // --- Generate triangles ---
        int ti = 0;

        for (int i = 0; i < segmentCount; i++)
        {
            int next = i + 1;

            if (closed)
                next %= pointCount;

            int vi = i * 2;
            int viNext = next * 2;

            triangles[ti++] = vi;
            triangles[ti++] = viNext;
            triangles[ti++] = vi + 1;

            triangles[ti++] = vi + 1;
            triangles[ti++] = viNext;
            triangles[ti++] = viNext + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}