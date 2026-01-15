using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnvironmentGrid))]
public class EnvironmentGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnvironmentGrid grid = (EnvironmentGrid)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Grid"))
        {
            grid.GenerateGrid();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            grid.ClearGrid();
        }
    }
}
