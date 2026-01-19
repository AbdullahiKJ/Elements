using System.Collections.Generic;

public class FireCluster
{
    public HashSet<EnvironmentGridCell> cells = new();

    public float propagationTimer;
    public float burnTimer;

    public float propagationInterval;
    public float burnDuration;

    // Constructor
    public FireCluster(float propagationInterval = 1f, float burnDuration = 6f)
    {
        this.propagationInterval = propagationInterval;
        this.burnDuration = burnDuration;
    }
}

public class FirePropagationSystem
{
    private List<FireCluster> clusters = new();
    private EnvironmentGrid grid;

    public FirePropagationSystem(EnvironmentGrid grid)
    {
        this.grid = grid;
    }

    public void RegisterBurningCell(EnvironmentGridCell cell)
    {
        if (IsRegistered(cell))
            return;

        FireCluster cluster = FindAdjacentCluster(cell);

        if (cluster == null)
            CreateNewCluster(cell);
        else
            cluster.cells.Add(cell);
    }

    private bool IsRegistered(EnvironmentGridCell cell)
    {
        foreach (var cluster in clusters)
        {
            foreach (var c in cluster.cells)
            {
                if (cell.gridPosition == c.gridPosition)
                    return true;
            }
        }
        return false;
    }

    private void CreateNewCluster(EnvironmentGridCell cell)
    {
        var cluster = new FireCluster();

        cluster.cells.Add(cell);
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

            if (cluster.propagationTimer >= cluster.propagationInterval)
            {
                TryPropagate(cluster);
                cluster.propagationTimer = 0f;
            }

            if (cluster.burnTimer >= cluster.burnDuration)
            {
                BurnOutCluster(cluster);
                clusters.RemoveAt(i);
            }
        }
    }


    private void TryPropagate(FireCluster cluster)
    {
        HashSet<EnvironmentGridCell> newCells = new();

        foreach (var cell in cluster.cells)
        {
            if (cell == null)
                continue;

            foreach (var neighbor in grid.GetNeighbors(cell))
            {
                if (neighbor == null)
                    continue;

                if (neighbor.surfaceType != SurfaceType.Grass)
                    continue;

                if (neighbor.IsBurning || neighbor.currentStatus == EnvironmentStatusType.Wet || neighbor.currentStatus == EnvironmentStatusType.Frozen)
                    continue;

                newCells.Add(neighbor);
            }
        }

        FireCluster newCluster = new();

        foreach (var cell in newCells)
        {
            grid.SetCellBurning(cell);
            newCluster.cells.Add(cell);
        }
        clusters.Add(newCluster);
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
            grid.ApplyVisuals(cell);
        }
    }

}
