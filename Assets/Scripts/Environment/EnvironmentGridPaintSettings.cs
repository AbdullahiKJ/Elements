using UnityEngine;

public enum PaintMode
{
    Surface,
    Status,
    Both
}

public class EnvironmentGridPaintSettings : MonoBehaviour
{
    [Header("Paint Mode")]
    public PaintMode paintMode = PaintMode.Surface;

    [Header("Surface Paint")]
    public SurfaceType surfaceToPaint;

    [Header("Status Paint")]
    public EnvironmentStatusType statusToPaint;

    [Header("Brush")]
    public float brushRadius = 1f;
}
