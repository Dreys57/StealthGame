using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorGenerator
{
    
    public List<Node> CreateCorridor(List<RoomNode> allNodesCollection, int corridorWidth)
    {
        List<Node> corridorList=new List<Node>();
        Queue<RoomNode> structureToCheck = new Queue<RoomNode>(allNodesCollection.OrderByDescending(node => node.TreeLayerIndex).ToList());

        while (structureToCheck.Count > 0)
        {
            RoomNode node = structureToCheck.Dequeue();

            if (node.ChildrenNodeList.Count == 0)
            {
                continue;
            }
            
            CorridorNode corridor = new CorridorNode(node.ChildrenNodeList[0], node.ChildrenNodeList[1], corridorWidth);
            
            corridorList.Add(corridor);
        }

        return corridorList;
    }
}
