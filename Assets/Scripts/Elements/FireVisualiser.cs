using UnityEngine;

public class FireVisualiser
{
    public Texture2D heightMapTexture;
    public Texture2D fireTexture;
    public bool fireMapDirty = false;
    EnvironmentGrid grid;

    public FireVisualiser(EnvironmentGrid grid)
    {
        this.grid = grid;
        InitializeTexture();
        InitializeFireVFX();
    }

    public void InitializeTexture()
    {
        // Get heightmap texture from terrain
        int hmResolution = grid.terrain.terrainData.heightmapResolution;
        float[,] heights = grid.terrain.terrainData.GetHeights(0, 0, hmResolution, hmResolution);

        heightMapTexture = new Texture2D(
            hmResolution,
            hmResolution,
            TextureFormat.RFloat,
            false
        );

        float highestPoint = grid.terrain.terrainData.bounds.max.y;
        float terrainSizeY = grid.terrain.terrainData.size.y;
        for (int z = 0; z < hmResolution; z++)
        {
            for (int x = 0; x < hmResolution; x++)
            {
                float h = heights[z, x] * terrainSizeY / highestPoint;
                heightMapTexture.SetPixel(x, z, new Color(h, 0, 0));
            }
        }
        heightMapTexture.Apply();

        // Assign the highest point to the vfx
        grid.fireVFX.SetFloat("TerrainMaxHeight", highestPoint);

        fireTexture = new Texture2D(
            grid.width,
            grid.height,
            TextureFormat.RFloat,
            false
        );

        for (int y = 0; y < grid.height; y++)
        {
            for (int x = 0; x < grid.width; x++)
            {
                fireTexture.SetPixel(x, y, new Color(0, 0, 0));
            }
        }

        fireTexture.Apply();
    }

    public void InitializeFireVFX()
    {
        grid.fireVFX.SetTexture("FireMap", fireTexture);
        grid.fireVFX.SetTexture("HeightMap", heightMapTexture);
        grid.fireVFX.SetVector3("GridOrigin", grid.origin);
        grid.fireVFX.SetVector2("GridSize", new Vector2(grid.width, grid.height));
        grid.fireVFX.SetFloat("CellSize", grid.cellSize);

        // Play the fire VFX
        grid.fireVFX.Play();
    }

    public void SetFireIntensity(EnvironmentGridCell cell, float intensity)
    {
        cell.fireIntensity = intensity;
        float textureValue = intensity > 0f ? 1f : 0f;
        fireTexture.SetPixel(cell.gridPosition.x, cell.gridPosition.y, new Color(textureValue, 0, 0));
        fireMapDirty = true;
    }

    public void UpdateFireTexture()
    {
        fireTexture.Apply();
        grid.fireVFX.SendEvent("Ignite");
    }
}