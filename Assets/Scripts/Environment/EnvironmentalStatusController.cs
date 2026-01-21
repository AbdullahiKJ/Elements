using UnityEngine;

public class EnvironmentalStatusController : MonoBehaviour
{
    [SerializeField] ReactionResolver reactionResolver;
    public ReactionResult ProcessElement(ElementData element, SurfaceType surfaceType, EnvironmentStatusType currentStatus)
    {
        if (reactionResolver.TryGetReaction(
            element.type,
            currentStatus,
            surfaceType,
            out ReactionResult result))
        {
            // Additional effects like visual or audio feedback can be triggered here

            // Return the new status
            return result;
        }

        // Return the same status if no reaction is found
        else
            return null;
    }
}