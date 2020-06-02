using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private Transform seeker;
    [SerializeField] private Transform target;
    
    private Grid grid;
    
    /*public Transform Seeker
    {
        get => seeker;
        set => seeker = value;
    }*/

    /*public Vector3 Target
    {
        get => target;
        set => target = value;
    }*/

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            FindPath(seeker.position, target.position);
        }
        
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        
        PathfindingNode startNode = grid.NodeFromWorldPoint(startPos);
        PathfindingNode targetNode = grid.NodeFromWorldPoint(targetPos);
        
        Heap<PathfindingNode> openList = new Heap<PathfindingNode>(grid.MaxSize);
        List<PathfindingNode> closedList = new List<PathfindingNode>();
        
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathfindingNode currentNode = openList.RemoveFirst();
            
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                sw.Stop();
                print("Path found:" + sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode, targetNode);
                return;
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
                }
            }
        }
    }

    void RetracePath(PathfindingNode startNode, PathfindingNode endNode)
    {
        List<PathfindingNode> path = new List<PathfindingNode>();
        
        PathfindingNode currentnode = endNode;

        while (currentnode != startNode)
        {
            path.Add(currentnode);
            currentnode = currentnode.Parent;
        }
        
        path.Reverse();

        grid.Path = path;
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
