using UnityEngine;

public enum ElementType
{
    Fire,
    Water,
    Earth,
    Air,
    Steam,
    Mud,
    Ice,
}

[CreateAssetMenu(fileName = "ElementData", menuName = "Scriptable Objects/ElementData")]
public class ElementData : ScriptableObject
{
    public ElementType type;
    public float intensity;
    public GameObject hitVFX;
}
