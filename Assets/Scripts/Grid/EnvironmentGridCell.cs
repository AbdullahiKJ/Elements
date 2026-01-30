using UnityEngine;

public class EnvironmentGridCell
{
    public Vector2Int gridPosition;
    public SurfaceType surfaceType;
    public EnvironmentStatusType currentStatus;
    public bool IsBurning => currentStatus == EnvironmentStatusType.Burning;
    public FireCluster fireCluster;
    public float fireIntensity = 0f;
}
