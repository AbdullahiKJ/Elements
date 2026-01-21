// using UnityEditor;
// using UnityEditor.EditorTools;
// using UnityEngine;

// [EditorTool("Environment Grid Paint Tool")]
// public class EnvironmentGridPaintTool : EditorTool
// {
//     private GUIContent _icon;

//     private void OnEnable()
//     {
//         _icon = new GUIContent()
//         {
//             text = "Grid Paint",
//             tooltip = "Paint Environment Grid Cells"
//         };
//     }

//     public override GUIContent toolbarIcon => _icon;

//     public override void OnToolGUI(EditorWindow window)
//     {
//         int controlID = GUIUtility.GetControlID(FocusType.Passive);
//         HandleUtility.AddDefaultControl(controlID);

//         if (!Selection.activeGameObject) return;

//         var grid = Selection.activeGameObject.GetComponent<EnvironmentGrid>();
//         var settings = Selection.activeGameObject.GetComponent<EnvironmentGridPaintSettings>();

//         if (grid == null || settings == null)
//             return;

//         HandleScenePainting(grid, settings);
//     }

//     private void HandleScenePainting(
//     EnvironmentGrid grid,
//     EnvironmentGridPaintSettings settings)
//     {
//         Event e = Event.current;

//         Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//         if (!Physics.Raycast(ray, out RaycastHit hit))
//             return;

//         // Draw brush
//         Handles.color = Color.yellow;
//         Handles.DrawWireDisc(hit.point, Vector3.up, settings.brushRadius);

//         if (e.type == EventType.MouseDrag && e.button == 0 && !e.alt)
//         {
//             PaintCells(grid, settings, hit.point);
//             e.Use();
//         }
//     }

//     private void PaintCells(
//         EnvironmentGrid grid,
//         EnvironmentGridPaintSettings settings,
//         Vector3 worldPos)
//     {
//         foreach (Transform child in grid.transform)
//         {
//             var cell = child.GetComponent<EnvironmentGridCell>();
//             if (cell == null) continue;

//             float dist = Vector3.Distance(worldPos, cell.transform.position);
//             if (dist > settings.brushRadius) continue;

//             Undo.RecordObject(cell, "Paint Grid Cell");

//             switch (settings.paintMode)
//             {
//                 case PaintMode.Surface:
//                     cell.surfaceType = settings.surfaceToPaint;
//                     break;

//                 case PaintMode.Status:
//                     cell.currentStatus = settings.statusToPaint;
//                     break;

//                 case PaintMode.Both:
//                     cell.surfaceType = settings.surfaceToPaint;
//                     cell.currentStatus = settings.statusToPaint;
//                     break;
//             }

//             grid.ApplyVisuals(cell);
//             EditorUtility.SetDirty(cell);
//         }
//     }

// }
