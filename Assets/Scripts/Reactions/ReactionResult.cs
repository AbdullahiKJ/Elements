using UnityEngine;

[System.Serializable]
public class ReactionResult
{
    public EnvironmentStatusType newStatus;
    public GameObject reactionVFX;
    public float duration;
    [Header("Debug")]
    public Material reactionMaterial;
}
