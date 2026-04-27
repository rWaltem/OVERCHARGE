using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SplineMeshGenerator : MonoBehaviour
{
    public SplineContainer splineContainer;

    [Header("Mesh Settings")]
    public int resolution = 50;
    public float width = 1f;

    [Header("Shape Settings")]
    public bool generatePlane = true;   // true = flat plane, false = 3D mesh
    public float thickness = 0.5f;      // extrusion depth when not plane

    private Mesh mesh;

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

        int sideMultiplier = generatePlane ? 1 : 2; // top/bottom
        int vertCount = pointCount * 2 * sideMultiplier;

        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];

        int segmentCount = closed ? resolution : resolution;
        int triangleCount = generatePlane ? segmentCount * 6 : segmentCount * 12;
        int[] triangles = new int[triangleCount];

        // Generate vertices
        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)resolution;

            Vector3 position = (Vector3)spline.EvaluatePosition(t);
            Vector3 tangent = ((Vector3)spline.EvaluateTangent(t)).normalized;
            Vector3 up = ((Vector3)spline.EvaluateUpVector(t)).normalized;
            Vector3 right = Vector3.Cross(up, tangent).normalized;

            Vector3 left = position - right * width * 0.5f;
            Vector3 rightPos = position + right * width * 0.5f;

            int vi = i * 2;

            if (generatePlane)
            {
                vertices[vi] = left;
                vertices[vi + 1] = rightPos;

                uvs[vi] = new Vector2(0, t);
                uvs[vi + 1] = new Vector2(1, t);
            }
            else
            {
                // top
                vertices[vi] = left + up * thickness * 0.5f;
                vertices[vi + 1] = rightPos + up * thickness * 0.5f;

                // bottom
                int offset = pointCount * 2;
                vertices[vi + offset] = left - up * thickness * 0.5f;
                vertices[vi + offset + 1] = rightPos - up * thickness * 0.5f;

                uvs[vi] = new Vector2(0, t);
                uvs[vi + 1] = new Vector2(1, t);
                uvs[vi + offset] = new Vector2(0, t);
                uvs[vi + offset + 1] = new Vector2(1, t);
            }
        }

        // Triangles
        int ti = 0;

        for (int i = 0; i < segmentCount; i++)
        {
            int next = i + 1;
            if (closed) next %= pointCount;

            int vi = i * 2;
            int viNext = next * 2;

            if (generatePlane)
            {
                triangles[ti++] = vi;
                triangles[ti++] = viNext;
                triangles[ti++] = vi + 1;

                triangles[ti++] = vi + 1;
                triangles[ti++] = viNext;
                triangles[ti++] = viNext + 1;
            }
            else
            {
                int offset = pointCount * 2;

                // top face
                triangles[ti++] = vi;
                triangles[ti++] = viNext;
                triangles[ti++] = vi + 1;

                triangles[ti++] = vi + 1;
                triangles[ti++] = viNext;
                triangles[ti++] = viNext + 1;

                // bottom face
                triangles[ti++] = vi + offset;
                triangles[ti++] = vi + offset + 1;
                triangles[ti++] = viNext + offset + 1;

                triangles[ti++] = vi + offset;
                triangles[ti++] = viNext + offset + 1;
                triangles[ti++] = viNext + offset;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Update MeshCollider if it exists
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }
}