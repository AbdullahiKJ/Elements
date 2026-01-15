using UnityEngine;

public class ElementSurface : MonoBehaviour, IElementReceiver
{
    public SurfaceType surfaceType;
    public EnvironmentStatusType currentStatus;
    public EnvironmentalStatusController statusController;

    public void ReceiveElement(ElementData element, Vector3 hitPoint)
    {
        // todo: get grid data if applicable
        currentStatus = statusController.ProcessElement(element, hitPoint, surfaceType, currentStatus);
    }
}