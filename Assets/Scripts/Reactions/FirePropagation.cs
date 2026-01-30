using System.Collections.Generic;
using UnityEngine;

public class FireCluster
{
    public HashSet<EnvironmentGridCell> cells = new();

    public float propagationTimer;
    public float burnTimer;

    public float propagationInterval;
    public float burnDuration;

    public bool hasPropagated;
    public float propagationSpeed;

    // Constructor
    public FireCluster(float propagationInterval = 5f, float burnDuration = 10f, float propagationSpeed = 2f)
    {
        this.propagationInterval = propagationInterval;
        this.burnDuration = burnDuration;
        this.propagationSpeed = propagationSpeed;
    }
}

public class FirePropagationSystem
{
    private List<FireCluster> clusters = new();
    private EnvironmentGrid grid;
    private FireVisualiser fireVisualiser;

    public FirePropagationSystem(EnvironmentGrid grid, FireVisualiser fireVisualiser)
    {
        this.grid = grid;
        this.fireVisualiser = fireVisualiser;
    }

    public void RegisterBurningCell(EnvironmentGridCell cell)
    {
        if (cell.IsBurning)
            return;

        FireCluster cluster = FindAdjacentCluster(cell);

        if (cluster == null)
            CreateNewCluster(cell);
        else
        {
            cluster.cells.Add(cell);
            cell.fireCluster = cluster;
        }

        // Set the fire intensity for the new cell
        fireVisualiser.SetFireIntensity(cell, 1f);
    }

    public void DeregisterBurningCell(EnvironmentGridCell cell)
    {
        var cluster = cell.fireCluster;
        if (cluster == null)
            return;

        // Remove the cell from the cluster and reassign the cell cluster to null
        cluster.cells.Remove(cell);
        cell.fireCluster = null;

        // Remove the cluster from the list of all clusters if empty
        if (cluster.cells.Count == 0)
            clusters.Remove(cluster);

        // Set the fire intensity for the new cell
        fireVisualiser.SetFireIntensity(cell, 0f);
    }

    private void CreateNewCluster(EnvironmentGridCell cell)
    {
        var cluster = new FireCluster();

        cluster.cells.Add(cell);
        cell.fireCluster = cluster;
        clusters.Add(cluster);
    }


    private FireCluster FindAdjacentCluster(EnvironmentGridCell cell)
    {
        foreach (var cluster in clusters)
        {
            foreach (var c in cluster.cells)
            {
                if (grid.AreNeighbors(cell, c))
                    return cluster;
            }
        }
        return null;
    }

    public void Tick(float deltaTime)
    {
        for (int i = clusters.Count - 1; i >= 0; i--)
        {
            var cluster = clusters[i];

            cluster.propagationTimer += deltaTime;
            cluster.burnTimer += deltaTime;

            // Update the fire intensity of each burning cell
            foreach (var cell in cluster.cells)
            {
                float newIntensity = cell.fireIntensity - deltaTime / cluster.burnDuration;
                newIntensity = Mathf.Max(0, newIntensity);

                if (grid.canTick)
                    fireVisualiser.SetFireIntensity(cell, newIntensity);
            }

            // Reset the grid tick flag
            if (grid.canTick)
            {
                grid.canTick = false;
                grid.Invoke("SetCanTick", grid.tickInterval);
            }

            if (cluster.propagationTimer >= cluster.propagationInterval && !cluster.hasPropagated)
            {
                TryPropagate(cluster);
                cluster.hasPropagated = true;
            }

            if (cluster.burnTimer >= cluster.burnDuration)
            {
                BurnOutCluster(cluster);
                clusters.RemoveAt(i);
            }
        }

        // Update the fire texure if any changes were made
        if (fireVisualiser.fireMapDirty)
        {
            fireVisualiser.UpdateFireTexture();
            fireVisualiser.fireMapDirty = false;
        }
    }


    private void TryPropagate(FireCluster cluster)
    {
        HashSet<EnvironmentGridCell> currentPool = new(cluster.cells);
        HashSet<EnvironmentGridCell> visited = new(cluster.cells);

        HashSet<EnvironmentGridCell> newCells = new();

        for (int i = 0; i < cluster.propagationSpeed; i++)
        {
            HashSet<EnvironmentGridCell> nextPool = new();

            foreach (var cell in currentPool)
            {
                if (cell == null)
                    continue;

                foreach (var neighbor in grid.GetNeighbors(cell))
                {
                    if (neighbor == null)
                        continue;

                    if (visited.Contains(neighbor))
                        continue;

                    if (neighbor.surfaceType != SurfaceType.Grass)
                        continue;

                    if (neighbor.IsBurning || neighbor.currentStatus == EnvironmentStatusType.Wet || neighbor.currentStatus == EnvironmentStatusType.Frozen)
                        continue;

                    newCells.Add(neighbor);
                    visited.Add(neighbor);
                    nextPool.Add(neighbor);
                }
            }

            if (nextPool.Count == 0)
                break;

            currentPool = nextPool;
        }

        if (newCells.Count > 0)
        {
            FireCluster newCluster = new();

            foreach (var cell in newCells)
            {
                grid.SetCellBurning(cell);
                newCluster.cells.Add(cell);
                cell.fireCluster = newCluster;
                fireVisualiser.SetFireIntensity(cell, 1f);
            }

            clusters.Add(newCluster);
        }
    }

    private void BurnOutCluster(FireCluster cluster)
    {
        if (cluster == null)
            return;

        foreach (var cell in cluster.cells)
        {
            if (cell == null)
                continue;

            cell.currentStatus = EnvironmentStatusType.None;
            cell.surfaceType = SurfaceType.Dirt;
            cell.fireCluster = null;
            ReactionResult burnResult = new ReactionResult
            {
                terrainLayer = 1,
            };
            grid.ApplyVisuals(cell, burnResult, true);

            // Set the fire intensity for the new cell
            fireVisualiser.SetFireIntensity(cell, 0f);
        }
    }

}
