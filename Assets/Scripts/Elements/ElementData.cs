using UnityEngine;

public enum ElementType
{
    Fire,
    Water,
    Earth,
    Ice,
}

[CreateAssetMenu(fileName = "ElementData", menuName = "Scriptable Objects/ElementData")]
public class ElementData : ScriptableObject
{
    public ElementType type;
    public int spreadRadius;
    public GameObject hitVFX;
}
