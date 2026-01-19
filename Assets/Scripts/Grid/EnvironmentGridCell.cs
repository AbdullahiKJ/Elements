using UnityEngine;

public class EnvironmentGridCell : MonoBehaviour, IElementReceiver
{
    public Vector2Int gridPosition;
    public SurfaceType surfaceType;
    public EnvironmentStatusType currentStatus;
    public bool IsBurning => currentStatus == EnvironmentStatusType.Burning;

    [Header("Visuals")]
    public Renderer groundRenderer;
    public EnvironmentalStatusController statusController;
    public EnvironmentGrid grid;

    public void ReceiveElement(ElementData element)
    {
        // For now, just debug
        Debug.Log($"{element.type} element hit {currentStatus} {surfaceType} cell at {gridPosition} ");
        EnvironmentStatusType newStatus = statusController.ProcessElement(element, surfaceType, currentStatus);

        // Send this information to the grid
        grid.OnCellHit(element, this, newStatus);
    }
}
