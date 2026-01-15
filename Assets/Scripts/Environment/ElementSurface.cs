using UnityEngine;

public class ElementSurface : MonoBehaviour, IElementReceiver
{
    public SurfaceType surfaceType;
    public EnvironmentalStatusController statusController;
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void ReceiveElement(ElementData element, Vector3 hitPoint)
    {
        // todo: get grid data if applicable
        statusController.ProcessElement(element, hitPoint, surfaceType, meshRenderer);
    }
}