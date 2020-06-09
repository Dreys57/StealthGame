using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Pathfinding : MonoBehaviour
{
    private Grid grid;

    private PathRequestManager requestManager;

    private Unit unit;

    private void Start()
    {
        grid = GetComponent<Grid>();

        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Debug.Log("name " + grid.name + " target " + targetPos);
        Vector3[] waypoints = new Vector3[0];

        bool pathSucess = false;

        PathfindingNode startNode = grid.NodeFromWorldPoint(startPos);
        PathfindingNode targetNode = grid.NodeFromWorldPoint(targetPos);

        startNode.Walkable = true;

        if (startNode.Walkable && targetNode.Walkable)
        {
            Heap<PathfindingNode> openList = new Heap<PathfindingNode>(grid.MaxSize);
            List<PathfindingNode> closedList = new List<PathfindingNode>();

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                PathfindingNode currentNode = openList.RemoveFirst();

                closedList.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSucess = true;
                    
                    break;
                }

                foreach (PathfindingNode neighbor in grid.GetNeighbors(currentNode))
                {
                    if (!neighbor.Walkable || closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);

                    if (newMovementCostToNeighbor < neighbor.GCost || !openList.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                        else
                        {
                            openList.UpdateItem(neighbor);
                        }
                    }
                }
            }
        }

        yield return null;

        if (pathSucess)
        {
            waypoints = RetracePath(startNode, targetNode);
            Debug.Log("waypoints " + waypoints.Length);
        }
        
        requestManager.FinishedProcessingPath(waypoints, pathSucess);
    }

    Vector3[] RetracePath(PathfindingNode startNode, PathfindingNode endNode)
    {
        List<PathfindingNode> path = new List<PathfindingNode>();
        
        PathfindingNode currentnode = endNode;

        while (currentnode != startNode)
        {
            path.Add(currentnode);
            currentnode = currentnode.Parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        
        Array.Reverse(waypoints);

        return waypoints;
    }

    Vector3[] SimplifyPath(List<PathfindingNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        
        Vector2  oldDirection = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 newDirection = new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);

            if (newDirection != oldDirection)
            {
                waypoints.Add(path[i - 1].WorldPosition);
            }

            oldDirection = newDirection;
        }

        return waypoints.ToArray();
    }

    int GetDistance(PathfindingNode nodeA, PathfindingNode nodeB)
    {
        int diagonalCost = 14;
        int linearCost = 10;
        
        int distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (distanceX > distanceY)
        {
            return diagonalCost * distanceY + 10 * (distanceX - distanceY);
        }
        else
        {
            return diagonalCost * distanceX + 10 * (distanceY - distanceX);
        }
    }
    
}
