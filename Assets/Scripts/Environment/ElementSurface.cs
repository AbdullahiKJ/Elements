using UnityEngine;

public class ElementSurface : MonoBehaviour, IElementReceiver
{
    public SurfaceType surfaceType;
    public EnvironmentStatusType currentStatus;
    public EnvironmentalStatusController statusController;

    public void ReceiveElement(ElementData element, Vector3 hitPoint)
    {
        ReactionResult newResult = statusController.ProcessElement(element, surfaceType, currentStatus);
        currentStatus = newResult.newStatus;
    }
}