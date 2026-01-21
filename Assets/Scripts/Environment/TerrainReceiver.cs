using UnityEngine;

public class TerrainReceiver : MonoBehaviour, IElementReceiver
{
    public EnvironmentGrid grid;
    public EnvironmentalStatusController statusController;
    public void ReceiveElement(ElementData elementData, Vector3 hitPoint)
    {
        if (grid.TryGetCellFromWorld(hitPoint, out var cell))
        {
            ReactionResult newResult = statusController.ProcessElement(elementData, cell.surfaceType, cell.currentStatus);
            grid.OnCellHit(elementData, cell, newResult);
        }
    }
}