#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrackSurfaceGenerator))]
public class TrackSurfaceGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrackSurfaceGenerator gen = (TrackSurfaceGenerator)target;

        if (GUILayout.Button("Generate Track Mesh"))
        {
            gen.GenerateMesh();

            // Ensure changes are saved in editor
            EditorUtility.SetDirty(gen);
        }
    }
}
#endif