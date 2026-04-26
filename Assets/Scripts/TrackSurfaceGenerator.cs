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
            splineContainer = gameObject.GetComponent<SplineContainer>();

            if (!splineContainer)
            {
                Debug.LogError("No SplineContainer found on this GameObject.");
            }
        }
    }

    public void GenerateMesh()
    {
        if (splineContainer == null) return;

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

        int vertCount = (resolution + 1) * 2;

        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] triangles = new int[resolution * 6];

        Spline spline = splineContainer.Spline;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            Vector3 position = (Vector3)spline.EvaluatePosition(t);
            Vector3 tangent = ((Vector3)spline.EvaluateTangent(t)).normalized;
            Vector3 up = ((Vector3)spline.EvaluateUpVector(t)).normalized;

            // Stable orientation that respects spline twist
            Vector3 right = Vector3.Cross(up, tangent).normalized;

            int vertIndex = i * 2;

            vertices[vertIndex] = position - right * width * 0.5f;
            vertices[vertIndex + 1] = position + right * width * 0.5f;

            uvs[vertIndex] = new Vector2(0, t);
            uvs[vertIndex + 1] = new Vector2(1, t);

            if (i < resolution)
            {
                int triIndex = i * 6;

                triangles[triIndex] = vertIndex;
                triangles[triIndex + 1] = vertIndex + 2;
                triangles[triIndex + 2] = vertIndex + 1;

                triangles[triIndex + 3] = vertIndex + 1;
                triangles[triIndex + 4] = vertIndex + 2;
                triangles[triIndex + 5] = vertIndex + 3;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}