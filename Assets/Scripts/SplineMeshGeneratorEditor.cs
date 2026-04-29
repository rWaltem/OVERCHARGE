#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineMeshGenerator))]
public class SplineMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SplineMeshGenerator gen = (SplineMeshGenerator)target;

        if (GUILayout.Button("Generate Track Mesh"))
        {
            gen.GenerateMesh();

            // Ensure changes are saved in editor
            EditorUtility.SetDirty(gen);
        }
    }
}
#endif