using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    public EnvironmentGridCell cellPrefab;
    public EnvironmentalStatusController statusController;
    private EnvironmentGridCell[,] grid;
    private FirePropagationSystem fireSystem;

    void Start()
    {
        fireSystem = new FirePropagationSystem(this);
        GetGrid();
    }

    void Update()
    {
        fireSystem.Tick(Time.deltaTime);
    }

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

                // Apply cell visuals
                ApplyVisuals(cell);

                // Set the controller
                cell.statusController = statusController;

                // Set the grid
                cell.grid = this;

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

    void GetGrid()
    {
        grid = new EnvironmentGridCell[width, height];

        EnvironmentGridCell[] cells = this.GetComponentsInChildren<EnvironmentGridCell>();
        foreach (var cell in cells)
        {
            grid[cell.gridPosition.x, cell.gridPosition.y] = cell;
        }
    }

    private static readonly Vector2Int[] NeighborOffsets4 =
   {
    new(1, 0),
    new(-1, 0),
    new(0, 1),
    new(0, -1)
    };

    // Get four cardinal neighbors
    public IEnumerable<EnvironmentGridCell> GetNeighbors(
        EnvironmentGridCell cell)
    {
        if (cell == null)
            yield break;

        foreach (var offset in NeighborOffsets4)
        {
            Vector2Int pos = cell.gridPosition + offset;

            if (IsWithinBounds(pos))
            {
                var neighbor = grid[pos.x, pos.y];
                if (neighbor != null)
                    yield return neighbor;
            }
        }
    }

    // Check if a grid position is within bounds
    private bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }


    public bool AreNeighbors(EnvironmentGridCell cellA, EnvironmentGridCell cellB)
    {
        Vector2Int delta = cellA.gridPosition - cellB.gridPosition;
        int dx = Mathf.Abs(delta.x);
        int dy = Mathf.Abs(delta.y);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    public void OnCellHit(ElementData element, EnvironmentGridCell cell, EnvironmentStatusType newStatus)
    {
        // Apply the new status type
        cell.currentStatus = newStatus;

        // Handle fire element interactions
        if (cell.surfaceType == SurfaceType.Grass && newStatus == EnvironmentStatusType.Burning)
            fireSystem.RegisterBurningCell(cell);

        // Apply visuals to the affected cell
        ApplyVisuals(cell);
    }

    public void SetCellBurning(EnvironmentGridCell cell)
    {
        cell.currentStatus = EnvironmentStatusType.Burning;
        ApplyVisuals(cell);
    }


    // Update a cell's visuals
    public void ApplyVisuals(EnvironmentGridCell cell)
    {
        Color baseColor = Color.white;

        switch (cell.surfaceType)
        {
            case SurfaceType.Grass:
                baseColor = Color.green;
                break;
            case SurfaceType.Dirt:
                baseColor = new Color(0.5f, 0.25f, 0.1f);
                break;
            case SurfaceType.Water:
                baseColor = Color.blue;
                break;
        }

        switch (cell.currentStatus)
        {
            case EnvironmentStatusType.Burning:
                baseColor = Color.red;
                break;
            case EnvironmentStatusType.Wet:
                baseColor *= 0.7f;
                break;
            case EnvironmentStatusType.Frozen:
                baseColor = Color.teal;
                break;
            case EnvironmentStatusType.Mud:
                baseColor = Color.brown;
                break;
            default:
                break;
        }

        var tempMaterial = new Material(cell.groundRenderer.sharedMaterial);
        tempMaterial.color = baseColor;
        cell.groundRenderer.sharedMaterial = tempMaterial;
    }

}
