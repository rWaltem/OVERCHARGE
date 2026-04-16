using UnityEngine;
using Dreamteck.Splines;

[ExecuteInEditMode]
public class ChargeHitBoxGenerator : MonoBehaviour
{
    public SplineComputer spline;
    public GameObject boxPrefab; // empty GO with BoxCollider

    public int segmentCount = 10;
    public float width = 3f;
    public float height = 2f;
    public float lengthPadding = 0.1f;
    public Vector3 offset;

    public Transform container;

    public bool updateBoxes = false;

    void Update()
    {
        if (updateBoxes)
        {
            Generate();
            updateBoxes = false;
        }
    }

    public void Generate()
    {
        // always find existing holder first
        GameObject holder = GameObject.Find("Generated Charge Boxes");

        if (holder == null)
        {
            holder = new GameObject("Generated Charge Boxes");
            holder.transform.SetParent(transform);
        }

        container = holder.transform;

        // clear only generated children safely
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            GameObject child = container.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        for (int i = 0; i < segmentCount; i++)
        {
            float t0 = (float)i / segmentCount;
            float t1 = (float)(i + 1) / segmentCount;

            Vector3 p0 = spline.EvaluatePosition(t0);
            Vector3 p1 = spline.EvaluatePosition(t1);

            Vector3 mid = (p0 + p1) * 0.5f;
            Vector3 dir = (p1 - p0).normalized;

            Quaternion rot = Quaternion.LookRotation(dir);

            Vector3 worldOffset = rot * offset;
            mid += worldOffset;

            GameObject go = Instantiate(boxPrefab, mid, rot, container);

            go.layer = LayerMask.NameToLayer("Charge Pad");

            float length = Vector3.Distance(p0, p1) + lengthPadding;

            BoxCollider col = go.GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(width, height, length);

            go.name = $"ChargeBox_{i}";
        }
    }
}
