using UnityEngine;
public enum StatusType
{
    None,
    Wet,
    Frozen,
    Burning,
    Mud
}

public abstract class StatusEffect
{
    public float duration;
    public abstract StatusType Type { get; }

    public virtual void OnApply(GameObject target) { }
    public virtual void OnTick(GameObject target) { }
    public virtual void OnRemove(GameObject target) { }
}