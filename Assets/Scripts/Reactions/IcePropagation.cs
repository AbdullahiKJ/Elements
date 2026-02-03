using System.Collections.Generic;

public class IcePropagationSystem
{
    private HashSet<EnvironmentGridCell> iceCluster = new();
    private EnvironmentGrid grid;

    public IcePropagationSystem(EnvironmentGrid grid)
    {
        this.grid = grid;
    }

    public void FreezeCell(EnvironmentGridCell cell)
    {
        if (iceCluster.Contains(cell))
            return;

        // Add the cell to the cluster and mark as frozen
        iceCluster.Add(cell);
        cell.currentStatus = EnvironmentStatusType.Frozen;

        // Try freezing characters
        FreezeCharacter(cell);

        // Instantly propagate ice from this cell
        InstantPropagate(cell);
    }

    private void InstantPropagate(EnvironmentGridCell startCell)
    {
        Queue<EnvironmentGridCell> queue = new();
        HashSet<EnvironmentGridCell> visited = new(iceCluster);

        queue.Enqueue(startCell);

        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();

            foreach (var neighbor in grid.GetNeighbors(cell))
            {
                if (neighbor == null)
                    continue;

                if (visited.Contains(neighbor))
                    continue;

                if (neighbor.surfaceType != SurfaceType.Grass || neighbor.currentStatus != EnvironmentStatusType.Wet)
                    continue;

                if (iceCluster.Contains(neighbor))
                    continue;

                // Freeze the neighbor and add to the cluster
                neighbor.currentStatus = EnvironmentStatusType.Frozen;
                iceCluster.Add(neighbor);
                visited.Add(neighbor);

                // Try freezing characters on this cell
                FreezeCharacter(neighbor);

                // Apply visuals
                ReactionResult newResult = new ReactionResult
                {
                    terrainLayer = 5,
                };
                grid.ApplyVisuals(neighbor, newResult);

                queue.Enqueue(neighbor);
            }
        }

        // Clear the ice cluster
        iceCluster.Clear();
    }

    void FreezeCharacter(EnvironmentGridCell cell)
    {
        foreach (var character in CharacterManager.instance.GetCharactersOnCell(cell, grid.width))
        {
            character.Freeze();
        }
    }
}