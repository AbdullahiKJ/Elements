using System;

[Serializable]
public struct ReactionKey : IEquatable<ReactionKey>
{
    public ElementType element;
    public StatusType status;

    public bool Equals(ReactionKey other)
    {
        return element == other.element && status == other.status;
    }

    public override bool Equals(object obj)
    {
        return obj is ReactionKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(element, status);
    }
}
