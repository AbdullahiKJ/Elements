using UnityEngine;

public class EnvironmentGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    public EnvironmentGridCell cellPrefab;

    private EnvironmentGridCell[,] grid;

    public void GenerateGrid()
    {
        ClearGrid();
        grid = new EnvironmentGridCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = transform.position +
                    new Vector3(x * cellSize, 0, y * cellSize);

                var cell = Instantiate(cellPrefab, worldPos, Quaternion.identity, transform);
                cell.gridPosition = new Vector2Int(x, y);
                cell.surfaceType = SurfaceType.Grass;
                cell.currentStatus = EnvironmentStatusType.None;

                // Set the renderer
                cell.groundRenderer = cell.GetComponent<Renderer>();

                // Apply visuals
                cell.ApplyVisuals();

                grid[x, y] = cell;
            }
        }
    }

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
