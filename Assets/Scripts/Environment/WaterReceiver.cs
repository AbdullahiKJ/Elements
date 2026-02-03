using System.Collections.Generic;
using UnityEngine;

public class WaterReceiver : MonoBehaviour, IElementReceiver
{
    public EnvironmentGrid grid;
    public EnvironmentalStatusController statusController;
    SurfaceType surfaceType = SurfaceType.Water;
    EnvironmentStatusType currentStatus = EnvironmentStatusType.None;
    GameObject iceVFXInstance;
    List<EnvironmentGridCell> waterCells = new();
    EnvironmentGridCell[,] gridCells;
    Collider waterCollider;


    void Start()
    {
        waterCollider = GetComponent<Collider>();

        // Get grid cells after a short delay to ensure the grid is initialised
        if (waterCells.Count == 0)
            Invoke(nameof(GetGridCellsBelow), 0.5f);
    }

    void GetGridCellsBelow()
    {
        grid.GetGrid(out gridCells);
        if (gridCells == null)
            return;

        for (int x = 0; x < gridCells.GetLength(0); x++)
        {
            for (int z = 0; z < gridCells.GetLength(1); z++)
            {
                EnvironmentGridCell cell = gridCells[x, z];
                if (cell.surfaceType == SurfaceType.Water)
                {
                    if (
                        waterCollider.bounds.max.x >= cell.worldPosition.x
                        && waterCollider.bounds.min.x <= cell.worldPosition.x
                        && waterCollider.bounds.max.z >= cell.worldPosition.z
                        && waterCollider.bounds.min.z <= cell.worldPosition.z
                        && cell.worldPosition.y <= transform.position.y)
                    {
                        waterCells.Add(cell);
                    }
                }
            }
        }
    }

    public void ReceiveElement(ElementData elementData, Vector3 hitPoint)
    {
        ReactionResult newResult = statusController.ProcessElement(elementData, surfaceType, currentStatus);
        if (newResult == null)
            return;

        currentStatus = newResult.newStatus;

        // Handle special case for freezing
        if (newResult.newStatus == EnvironmentStatusType.Frozen)
        {
            if (newResult.reactionVFX != null)
            {
                iceVFXInstance = Instantiate(newResult.reactionVFX, this.transform);
                // Set the scale of the ice VFX to match the water area
                Vector3 localSize = this.transform.InverseTransformVector(waterCollider.bounds.size);
                iceVFXInstance.transform.localScale = new Vector3(localSize.x, iceVFXInstance.transform.localScale.y, localSize.z);
                iceVFXInstance.transform.position = new Vector3(transform.position.x, iceVFXInstance.transform.position.y, transform.position.z);
            }

            // Set the new status for all child water cells
            foreach (var cell in waterCells)
            {
                cell.currentStatus = newResult.newStatus;
            }
        }

        // Handle special case for melting
        if (newResult.newStatus == EnvironmentStatusType.Wet)
        {
            // Create steam vfx if it exists at each cell
            if (newResult.reactionVFX != null)
            {
                foreach (var waterCell in waterCells)
                {
                    // Set the new status for all child water cells
                    waterCell.currentStatus = EnvironmentStatusType.None;

                    // Create steam
                    Vector3 spawnPosition = waterCell.worldPosition;
                    spawnPosition.y = transform.position.y;
                    Instantiate(newResult.reactionVFX, spawnPosition, Quaternion.identity);
                }
            }

            currentStatus = EnvironmentStatusType.None;

            // Destroy the ice VFX if it exists
            if (iceVFXInstance != null)
            {
                Destroy(iceVFXInstance);
            }
        }
    }
}