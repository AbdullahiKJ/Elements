using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEditor;

public class EnvironmentGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public Vector3 origin;

    public EnvironmentalStatusController statusController;
    private EnvironmentGridCell[,] grid;
    private FirePropagationSystem fireSystem;
    private FireVisualiser fireVisualiser;
    private IcePropagationSystem iceSystem;
    public float tickInterval = 1f;
    public bool canTick = true;
    public float editorOffset = 10f;

    [Header("Terrain Settings")]
    public Terrain terrain;
    TerrainData terrainData;
    Vector3 terrainPos;
    int alphamapWidth;
    int alphamapHeight;
    int heightmapResolution;
    int layers;
    float[,,] alphamaps;
    int[][,] detailMap;
    int detailMapLayerCount;

    [Header("Visual Effects")]
    public VisualEffect fireVFX;

    void Start()
    {
        fireVisualiser = new FireVisualiser(this);
        fireSystem = new FirePropagationSystem(this, fireVisualiser);
        iceSystem = new IcePropagationSystem(this);

        GenerateGrid();
        InitializeFromTerrain();

        // Register a function that resets the terrain when exiting play mode
        EditorApplication.playModeStateChanged += (PlayModeStateChange stateChange) =>
        {
            if (stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                // Reset terrain alphamaps (materials)
                terrainData.SetAlphamaps(0, 0, alphamaps);

                // Reset detail maps (grass)
                for (int i = 0; i < detailMapLayerCount; i++)
                {
                    terrainData.SetDetailLayer(0, 0, i, detailMap[i]);
                }
            }
        };
    }

    public void SetCanTick()
    {
        canTick = true;
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
                grid[x, y] = new EnvironmentGridCell
                {
                    gridPosition = new Vector2Int(x, y),
                    surfaceType = SurfaceType.Grass,
                    currentStatus = EnvironmentStatusType.None
                };
            }
        }

        InitializeFromTerrain();
    }

    public void ClearGrid()
    {
        grid = new EnvironmentGridCell[width, height];
    }

    public void GetGrid(out EnvironmentGridCell[,] outGrid)
    {
        outGrid = grid;
    }

    void InitializeFromTerrain()
    {
        if (terrain == null)
            return;

        terrainData = terrain.terrainData;
        terrainPos = terrain.transform.position;

        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;
        layers = terrainData.alphamapLayers;

        heightmapResolution = terrainData.heightmapResolution;

        alphamaps = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

        detailMapLayerCount = terrainData.detailPrototypes.Length;
        detailMap = new int[detailMapLayerCount][,];
        for (int i = 0; i < detailMapLayerCount; i++)
        {
            detailMap[i] = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, i);

        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // World position at cell center
                Vector3 worldPos = GetWorldFromCell(new Vector2Int(x, y));

                // Store the world position in the cell
                grid[x, y].worldPosition = worldPos;

                // Convert to terrain UV
                float normX = (worldPos.x - terrainPos.x) / terrainData.size.x;
                float normZ = (worldPos.z - terrainPos.z) / terrainData.size.z;

                int mapX = Mathf.Clamp(Mathf.RoundToInt(normX * (alphamapWidth - 1)), 0, alphamapWidth - 1);
                int mapZ = Mathf.Clamp(Mathf.RoundToInt(normZ * (alphamapHeight - 1)), 0, alphamapHeight - 1);

                (grid[x, y].surfaceType, grid[x, y].currentStatus) = DetermineSurface(alphamaps, mapX, mapZ);
            }
        }
    }

    void PaintCell(Vector2Int cellPos, int terrainLayer, bool removeGrass = false)
    {
        // World space bounds of the cell center
        Vector3 worldMin = GetWorldFromCell(cellPos, false);

        Vector3 worldMax = worldMin + new Vector3(cellSize, 0f, cellSize);

        // Convert world to normalized
        float normMinX = (worldMin.x - terrainPos.x) / terrainData.size.x;
        float normMaxX = (worldMax.x - terrainPos.x) / terrainData.size.x;
        float normMinZ = (worldMin.z - terrainPos.z) / terrainData.size.z;
        float normMaxZ = (worldMax.z - terrainPos.z) / terrainData.size.z;

        // Convert normalized vectors to alphamap indices
        int startX = Mathf.FloorToInt(normMinX * alphamapWidth);
        int startZ = Mathf.FloorToInt(normMinZ * alphamapHeight);

        int endX = Mathf.CeilToInt(normMaxX * alphamapWidth);
        int endZ = Mathf.CeilToInt(normMaxZ * alphamapHeight);

        startX = Mathf.Clamp(startX, 0, alphamapWidth - 1);
        startZ = Mathf.Clamp(startZ, 0, alphamapHeight - 1);
        endX = Mathf.Clamp(endX, 0, alphamapWidth);
        endZ = Mathf.Clamp(endZ, 0, alphamapHeight);

        int paintWidth = endX - startX;
        int paintHeight = endZ - startZ;

        float[,,] paintData = new float[paintHeight, paintWidth, layers];

        for (int z = 0; z < paintHeight; z++)
        {
            for (int x = 0; x < paintWidth; x++)
            {
                for (int l = 0; l < layers; l++)
                    paintData[z, x, l] = 0f;

                paintData[z, x, terrainLayer] = 1f;
            }
        }

        terrainData.SetAlphamaps(startX, startZ, paintData);

        // Remove grass if needed
        if (removeGrass)
        {
            int detailWidth = terrainData.detailWidth;
            int detailHeight = terrainData.detailHeight;

            startX = Mathf.FloorToInt(normMinX * detailWidth);
            startZ = Mathf.FloorToInt(normMinZ * detailHeight);
            endX = Mathf.CeilToInt(normMaxX * detailWidth);
            endZ = Mathf.CeilToInt(normMaxZ * detailHeight);

            startX = Mathf.Clamp(startX, 0, detailWidth - 1);
            startZ = Mathf.Clamp(startZ, 0, detailHeight - 1);
            endX = Mathf.Clamp(endX, 0, detailWidth);
            endZ = Mathf.Clamp(endZ, 0, detailHeight);

            int detailPaintWidth = endX - startX;
            int detailPaintHeight = endZ - startZ;

            for (int i = 0; i < detailMapLayerCount; i++)
            {
                int[,] details = terrainData.GetDetailLayer(
                    startX,
                    startZ,
                    detailPaintWidth,
                    detailPaintHeight,
                    i
                );

                for (int z = 0; z < detailPaintHeight; z++)
                    for (int x = 0; x < detailPaintWidth; x++)
                        details[z, x] = 0;

                terrainData.SetDetailLayer(
                    startX,
                    startZ,
                    i,
                    details
                );
            }
        }
    }

    (SurfaceType, EnvironmentStatusType) DetermineSurface(float[,,] alphamaps, int x, int z)
    {
        int strongestLayer = 0;
        float strongestWeight = 0f;

        for (int i = 0; i < alphamaps.GetLength(2); i++)
        {
            float weight = alphamaps[z, x, i];
            if (weight > strongestWeight)
            {
                strongestWeight = weight;
                strongestLayer = i;
            }
        }

        return strongestLayer switch
        {
            0 => (SurfaceType.Grass, EnvironmentStatusType.None),
            1 => (SurfaceType.Dirt, EnvironmentStatusType.None),
            2 => (SurfaceType.Water, EnvironmentStatusType.None),
            3 => (SurfaceType.Dirt, EnvironmentStatusType.Mud),
            _ => (SurfaceType.Grass, EnvironmentStatusType.None),
        };
    }

    public bool TryGetCellFromWorld(Vector3 worldPos, out EnvironmentGridCell cell)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - origin.z) / cellSize);

        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            cell = null;
            return false;
        }

        cell = grid[x, y];
        return true;
    }

    public Vector3 GetWorldFromCell(Vector2Int gridPos, bool getCenter = true)
    {
        if (gridPos == null)
            return Vector3.zero;

        int hmX = Mathf.RoundToInt((float)gridPos.x / (width - 1) * (heightmapResolution - 1));
        int hmZ = Mathf.RoundToInt((float)gridPos.y / (height - 1) * (heightmapResolution - 1));

        // World position at cell center
        Vector3 worldPos = origin + new Vector3(
            (gridPos.x + (getCenter ? 0.5f : 0f)) * cellSize,
            terrainData.GetHeight(hmX, hmZ) + terrain.transform.position.y,
            (gridPos.y + (getCenter ? 0.5f : 0f)) * cellSize
        );
        return worldPos;
    }

    private static readonly Vector2Int[] NeighborOffsets4 =
   {
    new(1, 0),
    new(-1, 0),
    new(0, 1),
    new(0, -1)
    };

    // Get four cardinal neighbors
    public IEnumerable<EnvironmentGridCell> GetNeighbors(EnvironmentGridCell cell)
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

    public void OnCellHit(ElementData element, EnvironmentGridCell cell, ReactionResult result, bool splash = true)
    {
        // Handle fire element interactions
        if (cell.surfaceType == SurfaceType.Grass && result != null && result.newStatus == EnvironmentStatusType.Burning)
            fireSystem.RegisterBurningCell(cell);
        else
            fireSystem.DeregisterBurningCell(cell);

        // Handle ice element interactions
        if (cell.surfaceType == SurfaceType.Grass && result != null && result.newStatus == EnvironmentStatusType.Frozen)
            iceSystem.FreezeCell(cell);

        // Apply the new status type
        if (result != null)
            cell.currentStatus = result.newStatus;

        // Apply visuals to the affected cell
        ApplyVisuals(cell, result);

        if (splash && element.spreadRadius != 0)
            ApplySplash(cell, element);
    }

    public void SetCellBurning(EnvironmentGridCell cell)
    {
        cell.currentStatus = EnvironmentStatusType.Burning;
        ReactionResult burningResult = new ReactionResult();
        ApplyVisuals(cell, burningResult);
    }


    // Update a cell's visuals
    public void ApplyVisuals(EnvironmentGridCell cell, ReactionResult result, bool removeGrass = false)
    {
        if (result == null)
            return;

        if (result.reactionVFX != null)
            Instantiate(result.reactionVFX, cell.worldPosition, Quaternion.identity);

        // Check if result has a terrain layer to paint
        if (result.terrainLayer >= 0)
            PaintCell(cell.gridPosition, result.terrainLayer, removeGrass);
    }

    void ApplySplash(EnvironmentGridCell hitCell, ElementData elementData)
    {
        foreach (var cell in GetCellsInRadius(hitCell, elementData.spreadRadius))
        {
            ReactionResult newResult = GetNewReaction(elementData, cell);
            OnCellHit(elementData, cell, newResult, false);
        }
    }

    ReactionResult GetNewReaction(ElementData elementData, EnvironmentGridCell cell)
    {
        return statusController.ProcessElement(elementData, cell.surfaceType, cell.currentStatus);
    }

    public IEnumerable<EnvironmentGridCell> GetCellsInRadius(EnvironmentGridCell center, int radius)
    {
        Vector2Int c = center.gridPosition;
        int radiusSq = radius * radius;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int distSq = x * x + y * y;
                if (distSq > radiusSq)
                    continue;

                Vector2Int pos = c + new Vector2Int(x, y);

                if (!IsWithinBounds(pos))
                    continue;

                var cell = grid[pos.x, pos.y];
                if (cell != null)
                    yield return cell;
            }
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - this.transform.position.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.z - this.transform.position.z) / cellSize);
        return new Vector2Int(x, y);
    }

    // Display grid in the editor
    void OnDrawGizmos()
    {
        if (grid == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = grid[x, y];
                if (cell == null)
                    continue;

                Vector3 pos = origin + new Vector3(
                    (x + 0.5f) * cellSize,
                    editorOffset,
                    (y + 0.5f) * cellSize
                );

                // Set color based on surface / status
                Gizmos.color = GetCellColor(cell);

                // Draw a cube or square
                Gizmos.DrawCube(pos, Vector3.one * (cellSize * 0.9f));
            }
        }
    }

    Color GetCellColor(EnvironmentGridCell cell)
    {
        Color baseColor = cell.surfaceType switch
        {
            SurfaceType.Grass => Color.green,
            SurfaceType.Dirt => new Color(0.5f, 0.25f, 0.1f),
            SurfaceType.Water => Color.blue,
            _ => Color.white
        };

        // Overlay status
        switch (cell.currentStatus)
        {
            case EnvironmentStatusType.Burning:
                baseColor = Color.red;
                break;
            case EnvironmentStatusType.Frozen:
                baseColor = Color.cyan;
                break;
            case EnvironmentStatusType.Wet:
                baseColor = Color.darkGreen;
                break;
            case EnvironmentStatusType.Mud:
                baseColor = Color.brown;
                break;
        }

        return baseColor;
    }
}
