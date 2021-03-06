﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode : IHeapItem<PathfindingNode>
{
    private bool walkable;
    
    private Vector3 worldPosition;

    private PathfindingNode parent;

    private int gCost;
    private int hCost;
    private int fCost;
    private int gridX;
    private int gridY;
    private int heapIndex;
    
    public PathfindingNode Parent
    {
        get => parent;
        set => parent = value;
    }

    public int GridX
    {
        get => gridX;
        set => gridX = value;
    }

    public int GridY
    {
        get => gridY;
        set => gridY = value;
    }

    public int FCost {get => gCost + hCost;}

    public int GCost
    {
        get => gCost;
        set => gCost = value;
    }

    public int HCost
    {
        get => hCost;
        set => hCost = value;
    }

    public bool Walkable
    {
        get => walkable;
        set => walkable = value;
    }

    public Vector3 WorldPosition {get => worldPosition;}

    public PathfindingNode(bool walkable_, Vector3 worldPosition_, int gridX_, int gridY_)
    {
        walkable = walkable_;

        worldPosition = worldPosition_;

        gridX = gridX_;

        gridY = gridY_;
    }

    public int HeapIndex
    {
        get { return heapIndex;}
        set { heapIndex = value; }
    }

    public int CompareTo(PathfindingNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
