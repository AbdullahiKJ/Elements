using UnityEngine;

public class EnvironmentalStatusController : MonoBehaviour
{
    public EnvironmentStatusType CurrentStatus { get; private set; }

    [SerializeField] ReactionResolver reactionResolver;
    public EnvironmentStatusType ProcessElement(ElementData element, Vector3 hitPoint, SurfaceType surfaceType, EnvironmentStatusType currentStatus)
    {
        if (reactionResolver.TryGetReaction(
            element.type,
            CurrentStatus,
            surfaceType,
            out ReactionResult result))
        {
            // Additional effects like visual or audio feedback can be triggered here

            // Return the new status
            return result.newStatus;
        }

        // Return the same status if no reaction is found
        else
            return currentStatus;
    }
}