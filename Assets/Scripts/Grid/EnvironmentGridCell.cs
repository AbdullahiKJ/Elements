using UnityEngine;

public class EnvironmentGridCell : MonoBehaviour, IElementReceiver
{
    public Vector2Int gridPosition;
    public SurfaceType surfaceType;
    public EnvironmentStatusType currentStatus;

    [Header("Visuals")]
    public Renderer groundRenderer;
    public EnvironmentalStatusController statusController;

    public void ReceiveElement(ElementData element, Vector3 hitPoint)
    {
        // For now, just debug
        Debug.Log($"Element {element.type} hit cell {gridPosition}");
        currentStatus = statusController.ProcessElement(element, hitPoint, surfaceType, currentStatus);
        ApplyVisuals();
    }

    public void ApplyVisuals()
    {
        Color baseColor = Color.white;

        switch (surfaceType)
        {
            case SurfaceType.Grass:
                baseColor = Color.green;
                break;
            case SurfaceType.Dirt:
                baseColor = new Color(0.5f, 0.25f, 0.1f);
                break;
            case SurfaceType.Water:
                baseColor = Color.blue;
                break;
        }

        switch (currentStatus)
        {
            case EnvironmentStatusType.Burning:
                baseColor = Color.red;
                break;
            case EnvironmentStatusType.Wet:
                baseColor *= 0.7f;
                break;
            case EnvironmentStatusType.Frozen:
                baseColor = Color.teal;
                break;
            case EnvironmentStatusType.Mud:
                baseColor = Color.brown;
                break;
            default:
                break;
        }

        var tempMaterial = new Material(groundRenderer.sharedMaterial);
        tempMaterial.color = baseColor;
        groundRenderer.sharedMaterial = tempMaterial;
    }
}
