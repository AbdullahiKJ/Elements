using System;

[Serializable]
public struct ReactionKey : IEquatable<ReactionKey>
{
    public ElementType element;
    public EnvironmentStatusType status;
    public SurfaceType surfaceType;

    public bool Equals(ReactionKey other)
    {
        return element == other.element && status == other.status && surfaceType == other.surfaceType;
    }

    public override bool Equals(object obj)
    {
        return obj is ReactionKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(element, status, surfaceType);
    }
}
