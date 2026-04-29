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
    public bool generatePlane = true;
    public float thickness = 0.5f;

    [Header("Mobius Settings")]
    public bool mobius = false;
    [Range(0f, 360f)]
    public float mobiusRotation = 180f;

    public enum UVMode
    {
        Stretch,
        Tile,
        WorldDistance
    }

    [Header("UV Settings")]
    public UVMode uvMode = UVMode.Stretch;
    public float uvTiling = 1f;
    public float uvOffset = 0f;
    public bool flipU = false;
    public bool flipV = false;

    private Mesh mesh;

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

        int sideMultiplier = generatePlane ? 1 : 2;
        int vertCount = pointCount * 2 * sideMultiplier;

        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];

        int segmentCount = resolution;
        int triangleCount = generatePlane ? segmentCount * 6 : segmentCount * 12;
        int[] triangles = new int[triangleCount];

        float[] distances = new float[pointCount];
        float totalLength = 0f;

        if (uvMode == UVMode.WorldDistance)
        {
            Vector3 prev = (Vector3)spline.EvaluatePosition(0f);

            for (int i = 1; i < pointCount; i++)
            {
                float tDist = i / (float)resolution;
                Vector3 pos = (Vector3)spline.EvaluatePosition(tDist);

                totalLength += Vector3.Distance(prev, pos);
                distances[i] = totalLength;

                prev = pos;
            }
        }

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

            float v;
            switch (uvMode)
            {
                case UVMode.Stretch:
                    v = t;
                    break;
                case UVMode.Tile:
                    v = t * uvTiling;
                    break;
                case UVMode.WorldDistance:
                    v = distances[i] * uvTiling;
                    break;
                default:
                    v = t;
                    break;
            }

            v += uvOffset;

            float u0 = flipU ? 1f : 0f;
            float u1 = flipU ? 0f : 1f;

            if (flipV) v = -v;

            if (generatePlane)
            {
                vertices[vi]     = left;
                vertices[vi + 1] = rightPos;

                uvs[vi]     = new Vector2(u0, v);
                uvs[vi + 1] = new Vector2(u1, v);
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

                uvs[vi]              = new Vector2(u0, v);
                uvs[vi + 1]          = new Vector2(u1, v);
                uvs[vi + offset]     = new Vector2(u0, v);
                uvs[vi + offset + 1] = new Vector2(u1, v);
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

                // top
                triangles[ti++] = vi;
                triangles[ti++] = viNext;
                triangles[ti++] = vi + 1;

                triangles[ti++] = vi + 1;
                triangles[ti++] = viNext;
                triangles[ti++] = viNext + 1;

                // bottom
                triangles[ti++] = vi + offset;
                triangles[ti++] = vi + offset + 1;
                triangles[ti++] = viNext + offset + 1;

                triangles[ti++] = vi + offset;
                triangles[ti++] = viNext + offset + 1;
                triangles[ti++] = viNext + offset;
            }
        }

        // Mobius seam
        if (mobius && !closed)
        {
            Vector3 seamTangent = ((Vector3)spline.EvaluateTangent(0f)).normalized;
            Vector3 seamCenter  = (Vector3)spline.EvaluatePosition(0f);

            int lastRing  = (resolution - 1) * 2;
            int firstRing = 0;

            if (generatePlane)
            {
                Vector3 r0L = RotateAround(vertices[firstRing],     seamCenter, seamTangent, mobiusRotation);
                Vector3 r0R = RotateAround(vertices[firstRing + 1], seamCenter, seamTangent, mobiusRotation);

                System.Array.Resize(ref vertices, vertCount + 2);
                System.Array.Resize(ref uvs,      vertCount + 2);

                vertices[vertCount]     = r0L;
                vertices[vertCount + 1] = r0R;

                uvs[vertCount]     = uvs[firstRing];
                uvs[vertCount + 1] = uvs[firstRing + 1];

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

                System.Array.Resize(ref vertices, vertCount + 4);
                System.Array.Resize(ref uvs,      vertCount + 4);

                vertices[vertCount]     = r0L;
                vertices[vertCount + 1] = r0R;
                vertices[vertCount + 2] = r0BL;
                vertices[vertCount + 3] = r0BR;

                uvs[vertCount]     = uvs[firstRing];
                uvs[vertCount + 1] = uvs[firstRing + 1];
                uvs[vertCount + 2] = uvs[firstRing + offset];
                uvs[vertCount + 3] = uvs[firstRing + offset + 1];

                int seam = triangles.Length - 12;
                int vL  = lastRing;
                int vR  = lastRing + 1;
                int vBL = lastRing + offset;
                int vBR = lastRing + offset + 1;

                triangles[seam]     = vL;
                triangles[seam + 1] = vertCount;
                triangles[seam + 2] = vR;
                triangles[seam + 3] = vR;
                triangles[seam + 4] = vertCount;
                triangles[seam + 5] = vertCount + 1;

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

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            GenerateMesh();
        }
    }
#endif
}