using UnityEngine;

public class EnvironmentalStatusController : MonoBehaviour
{
    public EnvironmentStatusType CurrentStatus { get; private set; }

    [SerializeField] ReactionResolver reactionResolver;
    public void ProcessElement(ElementData element, Vector3 hitPoint, SurfaceType surfaceType, MeshRenderer renderer)
    {
        if (reactionResolver.TryGetReaction(
            element.type,
            CurrentStatus,
            surfaceType,
            out ReactionResult result))
        {
            ApplyStatusEffect(result.newStatus);
            // Additional effects like visual or audio feedback can be triggered here

            // Testing
            renderer.material = result.reactionMaterial;
        }
    }

    void ApplyStatusEffect(EnvironmentStatusType? newStatus)
    {
        if (newStatus.HasValue)
            CurrentStatus = newStatus.Value;
    }
}