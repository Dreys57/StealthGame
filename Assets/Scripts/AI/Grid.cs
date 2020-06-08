using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Vector2 gridWorldSize;

    public Vector2 GridWorldSize
    {
        get => gridWorldSize;
        set => gridWorldSize = value;
    }

    [SerializeField] private float nodeRadius;

    private int maxSize;

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    [SerializeField] private LayerMask obstacleMask;
    
    private PathfindingNode[,] grid;

    private float nodeDiameter;
    
    private int gridSizeX;

    private int gridSizeY;
    
    public int GridSizeX => gridSizeX;

    public int GridSizeY => gridSizeY;

    private bool FinishedLevelGeneration = false;

    public bool FinishedLevelGeneration1
    {
        get => FinishedLevelGeneration;
        set => FinishedLevelGeneration = value;
    }


    private void Start()
    {
        nodeDiameter = nodeRadius * 2;

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new PathfindingNode[gridSizeX,gridSizeY];
        
        Vector3 worldBottomLeft =
            transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + 
                                     Vector3.right * (x * nodeDiameter + nodeRadius) +
                                     Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask));
                
                grid[x, y] = new PathfindingNode(walkable, worldPoint, x, y);
            }
        }
    }

    public List<PathfindingNode> GetNeighbors(PathfindingNode node)
    {
        List<PathfindingNode> neighbors = new List<PathfindingNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.GridX + x;
                int checkY = node.GridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public PathfindingNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (PathfindingNode node in grid)
            {
                Gizmos.color = (node.Walkable) ? Color.white : Color.red;

                Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
