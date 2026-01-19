using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnvironmentGridCell))]
public class EnvironmentGridCellEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnvironmentGridCell cell = (EnvironmentGridCell)target;

        EditorGUILayout.LabelField("Grid Position", cell.gridPosition.ToString());

        cell.surfaceType = (SurfaceType)EditorGUILayout.EnumPopup(
            "Surface Type", cell.surfaceType);

        cell.currentStatus = (EnvironmentStatusType)EditorGUILayout.EnumPopup(
            "Current Status", cell.currentStatus);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cell);
        }
    }
}
