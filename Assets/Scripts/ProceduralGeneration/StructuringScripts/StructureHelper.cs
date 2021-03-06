﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StructureHelper
{
    public static List<Node> TraverseGraphToExtractLowestLeaf(Node parentNode)
    {
        Queue<Node> nodesToCheck = new Queue<Node>();
        
        List<Node> listToReturn = new List<Node>();

        if (parentNode.ChildrenNodeList.Count == 0)
        {
            return  new List<Node>(){ parentNode };
        }

        foreach (Node child in parentNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(child);
        }

        while (nodesToCheck.Count > 0)
        {
            Node currentNode = nodesToCheck.Dequeue();

            if (currentNode.ChildrenNodeList.Count == 0)
            {
                listToReturn.Add(currentNode);
            }
            else
            {
                foreach (Node child in currentNode.ChildrenNodeList)
                {
                    nodesToCheck.Enqueue(child);
                }
            }
        }

        return listToReturn;
    }

    public static Vector2Int GenerateBottomLeftCornerBetween(Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;
        
        return new Vector2Int(Random.Range(minX, (int) (minX + (maxX - minX) * pointModifier)), Random.Range(minY, (int)(minY+ (maxY - minY) * pointModifier)));
    }
    
    public static Vector2Int GenerateTopRightCornerBetween(Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;
        
        return new Vector2Int(Random.Range((int) (minX + (maxX - minX) * pointModifier), maxX), Random.Range((int)(minY+ (maxY - minY) * pointModifier), maxY));
    }

    public static Vector2Int CalculateCenter(Vector2Int v1, Vector2Int v2)
    {
        Vector2 sum = v1 + v2;
        Vector2 vectorTemp = sum / 2;
        
        return new Vector2Int((int) vectorTemp.x, (int) vectorTemp.y);
    }
}

public enum RelativePosition
{
    UP,
    DOWN,
    RIGHT,
    LEFT
}
