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

    [Header("Mobius Settings")]
    public bool mobius = false;
    [Range(0f, 360f)]
    public float mobiusRotation = 180f;

    private Mesh mesh;

    // Rotate a point around a pivot along a given axis
    private Vector3 RotateAround(Vector3 point, Vector3 pivot, Vector3 axis, float angleDeg)
    {
        return pivot + Quaternion.AngleAxis(angleDeg, axis) * (point - pivot);
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
            Vector3 tangent  = ((Vector3)spline.EvaluateTangent(t)).normalized;
            Vector3 up       = ((Vector3)spline.EvaluateUpVector(t)).normalized;
            Vector3 right    = Vector3.Cross(up, tangent).normalized;

            Vector3 left     = position - right * width * 0.5f;
            Vector3 rightPos = position + right * width * 0.5f;

            int vi = i * 2;

            if (generatePlane)
            {
                vertices[vi]     = left;
                vertices[vi + 1] = rightPos;

                uvs[vi]     = new Vector2(0, t);
                uvs[vi + 1] = new Vector2(1, t);
            }
            else
            {
                // top
                vertices[vi]     = left     + up * thickness * 0.5f;
                vertices[vi + 1] = rightPos + up * thickness * 0.5f;

                // bottom
                int offset = pointCount * 2;
                vertices[vi + offset]     = left     - up * thickness * 0.5f;
                vertices[vi + offset + 1] = rightPos - up * thickness * 0.5f;

                uvs[vi]              = new Vector2(0, t);
                uvs[vi + 1]          = new Vector2(1, t);
                uvs[vi + offset]     = new Vector2(0, t);
                uvs[vi + offset + 1] = new Vector2(1, t);
            }
        }

        // Triangles
        int ti = 0;

        for (int i = 0; i < segmentCount; i++)
        {
            int next = i + 1;
            if (closed) next %= pointCount;

            int vi     = i    * 2;
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

        // Mobius seam: replace the closing segment's target vertices with
        // rotated copies of ring 0, so the strip twists at the seam.
        if (mobius && !closed)
        {
            Vector3 seamTangent = ((Vector3)spline.EvaluateTangent(0f)).normalized;
            Vector3 seamCenter  = (Vector3)spline.EvaluatePosition(0f);

            int lastRing  = (resolution - 1) * 2;
            int firstRing = 0;

            if (generatePlane)
            {
                // Build rotated copies of ring-0 vertices
                Vector3 r0L = RotateAround(vertices[firstRing],     seamCenter, seamTangent, mobiusRotation);
                Vector3 r0R = RotateAround(vertices[firstRing + 1], seamCenter, seamTangent, mobiusRotation);

                // Append two extra vertices
                System.Array.Resize(ref vertices, vertCount + 2);
                System.Array.Resize(ref uvs,      vertCount + 2);

                vertices[vertCount]     = r0L;
                vertices[vertCount + 1] = r0R;
                uvs[vertCount]          = new Vector2(0, 1f);
                uvs[vertCount + 1]      = new Vector2(1, 1f);

                // Retarget the last 6 triangle indices (the seam quad)
                int seam = triangles.Length - 6;
                triangles[seam]     = lastRing;
                triangles[seam + 1] = vertCount;
                triangles[seam + 2] = lastRing + 1;

                triangles[seam + 3] = lastRing + 1;
                triangles[seam + 4] = vertCount;
                triangles[seam + 5] = vertCount + 1;
            }
            else
            {
                int offset = pointCount * 2;

                Vector3 r0L  = RotateAround(vertices[firstRing],              seamCenter, seamTangent, mobiusRotation);
                Vector3 r0R  = RotateAround(vertices[firstRing + 1],          seamCenter, seamTangent, mobiusRotation);
                Vector3 r0BL = RotateAround(vertices[firstRing + offset],     seamCenter, seamTangent, mobiusRotation);
                Vector3 r0BR = RotateAround(vertices[firstRing + offset + 1], seamCenter, seamTangent, mobiusRotation);

                // Append four extra vertices (top-L, top-R, bot-L, bot-R)
                System.Array.Resize(ref vertices, vertCount + 4);
                System.Array.Resize(ref uvs,      vertCount + 4);

                vertices[vertCount]     = r0L;
                vertices[vertCount + 1] = r0R;
                vertices[vertCount + 2] = r0BL;
                vertices[vertCount + 3] = r0BR;

                uvs[vertCount]     = new Vector2(0, 1f);
                uvs[vertCount + 1] = new Vector2(1, 1f);
                uvs[vertCount + 2] = new Vector2(0, 1f);
                uvs[vertCount + 3] = new Vector2(1, 1f);

                // Retarget the last 12 triangle indices (top + bottom seam faces)
                int seam = triangles.Length - 12;
                int vL  = lastRing;
                int vR  = lastRing + 1;
                int vBL = lastRing + offset;
                int vBR = lastRing + offset + 1;

                // top face
                triangles[seam]     = vL;
                triangles[seam + 1] = vertCount;
                triangles[seam + 2] = vR;
                triangles[seam + 3] = vR;
                triangles[seam + 4] = vertCount;
                triangles[seam + 5] = vertCount + 1;

                // bottom face
                triangles[seam + 6]  = vBL;
                triangles[seam + 7]  = vBR;
                triangles[seam + 8]  = vertCount + 3;
                triangles[seam + 9]  = vBL;
                triangles[seam + 10] = vertCount + 3;
                triangles[seam + 11] = vertCount + 2;
            }
        }

        mesh.vertices  = vertices;
        mesh.triangles = triangles;
        mesh.uv        = uvs;

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