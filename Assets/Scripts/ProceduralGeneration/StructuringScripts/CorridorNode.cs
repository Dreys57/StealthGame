using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CorridorNode : Node
{
    private Node structure1;
    private Node structure2;
    
    private int corridorWidth;
    private int wallDistanceModifier = 1;

    private int checkValue = -1;

    public CorridorNode(Node structure1, Node structure2, int corridorWidth) : base(null) //base(null) so that corridors won't connect to each other 
    {
        this.structure1 = structure1;
        this.structure2 = structure2;
        this.corridorWidth = corridorWidth;
        
        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        RelativePosition relativePositionOfStructure2 = CheckPositionBetweenTwoStructures();

        switch (relativePositionOfStructure2)
        {
            case RelativePosition.UP:

                ProcessRoomRelationUpOrDown(this.structure1, this.structure2);
                
                break;
            case RelativePosition.DOWN:

                ProcessRoomRelationUpOrDown(this.structure2, this.structure1);
                
                break;
            case RelativePosition.RIGHT:

                ProcessRoomRelationRightOrLeft(this.structure1, this.structure2);
                
                break;
            case RelativePosition.LEFT:

                ProcessRoomRelationRightOrLeft(this.structure2, this.structure1);
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessRoomRelationRightOrLeft(Node structure1, Node structure2)
    {
        Node leftStructure = null;
        List<Node> leftStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeaf(structure1);
        
        Node rightStructure = null;
        List<Node> rightStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeaf(structure2);
        
        //We extracted all the children from the two structures before ordering the children list by the top right area for the left structure

        List<Node> sortedLeftStructure = leftStructureChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();

        if (sortedLeftStructure.Count == 1)
        {
            leftStructure = sortedLeftStructure[0];
        }
        else
        {
            // we then choose one of the children to be the left structure
            int maxX = sortedLeftStructure[0].TopRightAreaCorner.x;
            
            sortedLeftStructure = sortedLeftStructure.Where(children => Math.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();

            int index = Random.Range(0, sortedLeftStructure.Count);

            leftStructure = sortedLeftStructure[index];
        }
        
        // Using the left structure we just found, we can search for possible neighbors for the left one in the right structure
        List<Node> rightStructurePossibleNeighbors = rightStructureChildren.Where(child =>
            GetYForNeighbor(leftStructure.TopRightAreaCorner, leftStructure.BottomRightAreaCorner, 
                child.TopLeftAreaCorner, child.BottomLeftAreaCorner) != -1)
            .OrderBy(child => child.BottomRightAreaCorner.x).ToList();

        if (rightStructurePossibleNeighbors.Count <= 0)
        {
            rightStructure = structure2;
        }
        else
        {
            //And we take the first one in the list
            rightStructure = rightStructurePossibleNeighbors[0];
        }
        
        //Y will give us where the corridor has to be placed on the Z-Axis on the side of the structure
        int y = GetYForNeighbor(leftStructure.TopLeftAreaCorner, leftStructure.BottomRightAreaCorner,
            rightStructure.TopLeftAreaCorner, rightStructure.BottomLeftAreaCorner);

        while (y == checkValue && sortedLeftStructure.Count > 1)
        {
            //we pass through the sorted structure to see which one is the best candidate for the second end of the corridor
            sortedLeftStructure = sortedLeftStructure.Where(child => child.TopLeftAreaCorner.y != leftStructure.TopLeftAreaCorner.y).ToList();

            leftStructure = sortedLeftStructure[0];
            
            y = GetYForNeighbor(leftStructure.TopLeftAreaCorner, leftStructure.BottomRightAreaCorner,
                rightStructure.TopLeftAreaCorner, rightStructure.BottomLeftAreaCorner);
        }
        
        BottomLeftAreaCorner = new Vector2Int(leftStructure.BottomRightAreaCorner.x, y);
        TopRightAreaCorner = new Vector2Int(rightStructure.TopLeftAreaCorner.x, y + this.corridorWidth);
    }
    
    //This function gives us a Y depending on the node position to the other
    private int GetYForNeighbor(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return StructureHelper.CalculateCenter(rightNodeDown + new Vector2Int(0, wallDistanceModifier), 
                rightNodeUp - new Vector2Int(0, wallDistanceModifier + this.corridorWidth)).y;
        }

        if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateCenter(leftNodeDown + new Vector2Int(0, wallDistanceModifier), 
                leftNodeUp - new Vector2Int(0, wallDistanceModifier + this.corridorWidth)).y;
        }

        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateCenter(rightNodeDown + new Vector2Int(0, wallDistanceModifier), 
                leftNodeUp - new Vector2Int(0, wallDistanceModifier + this.corridorWidth)).y;
        }

        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateCenter(leftNodeDown + new Vector2Int(0, wallDistanceModifier), 
                rightNodeUp - new Vector2Int(0, wallDistanceModifier + this.corridorWidth)).y;
        }

        return checkValue;
    }
    
    //This is the same idea as the left/right function just above, but on the X-Axis
    private void ProcessRoomRelationUpOrDown(Node structure1, Node structure2)
    {
        Node bottomStructure = null;
        List<Node> structureBottomChildren = StructureHelper.TraverseGraphToExtractLowestLeaf(structure1);

        Node topStructure = null;
        List<Node> structureTopChildren = StructureHelper.TraverseGraphToExtractLowestLeaf(structure2);

        List<Node> sortedBottomStructure = structureBottomChildren.OrderByDescending(child => child.TopRightAreaCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottomChildren[0];
        }
        else
        {
            int maxY = sortedBottomStructure[0].TopLeftAreaCorner.y;

            sortedBottomStructure = sortedBottomStructure.Where(child => Mathf.Abs(maxY - child.TopLeftAreaCorner.y) < 10).ToList();

            int index = Random.Range(0, sortedBottomStructure.Count);

            bottomStructure = sortedBottomStructure[index];
        }

        List<Node> topStructurePossibleNeighbors = structureTopChildren
            .Where(child => GetXForNeighbor(bottomStructure.TopLeftAreaCorner, bottomStructure.TopRightAreaCorner,
                child.BottomLeftAreaCorner, child.BottomRightAreaCorner) != 1).OrderBy(child => child.BottomRightAreaCorner.y).ToList();

        if (topStructurePossibleNeighbors.Count == 0)
        {
            topStructure = structure2;
        }
        else
        {
            topStructure = topStructurePossibleNeighbors[0];
        }

        int x = GetXForNeighbor(bottomStructure.TopLeftAreaCorner, bottomStructure.TopRightAreaCorner, topStructure.BottomLeftAreaCorner, topStructure.BottomRightAreaCorner);

        while (x == checkValue && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure.Where(child => child.TopLeftAreaCorner.x != topStructure.TopLeftAreaCorner.x).ToList();

            bottomStructure = sortedBottomStructure[0];
            
            x = GetXForNeighbor(bottomStructure.TopLeftAreaCorner, bottomStructure.TopRightAreaCorner, topStructure.BottomLeftAreaCorner, topStructure.BottomRightAreaCorner);
        }
        
        BottomLeftAreaCorner = new Vector2Int(x, bottomStructure.TopLeftAreaCorner.y);
        TopRightAreaCorner = new Vector2Int(x + this.corridorWidth, topStructure.BottomLeftAreaCorner.y);
    }

    private int GetXForNeighbor(Vector2Int bottomNodeLeft, Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return StructureHelper.CalculateCenter(bottomNodeLeft + new Vector2Int(wallDistanceModifier, 0), bottomNodeRight - new Vector2Int(this.corridorWidth + wallDistanceModifier, 0)).x;
        }
        
        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return StructureHelper.CalculateCenter(topNodeLeft + new Vector2Int(wallDistanceModifier, 0), topNodeRight - new Vector2Int(this.corridorWidth + wallDistanceModifier, 0)).x;
        }
        
        if (bottomNodeLeft.x >= topNodeLeft.x && bottomNodeLeft.x <= topNodeRight.x)
        {
            return StructureHelper.CalculateCenter(bottomNodeLeft + new Vector2Int(wallDistanceModifier, 0), topNodeRight - new Vector2Int(this.corridorWidth + wallDistanceModifier, 0)).x;
        }
        
        if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return StructureHelper.CalculateCenter(topNodeLeft + new Vector2Int(wallDistanceModifier, 0), bottomNodeRight - new Vector2Int(this.corridorWidth + wallDistanceModifier, 0)).x;
        }

        return checkValue;
    }

    private RelativePosition CheckPositionBetweenTwoStructures()
    {
        Vector2 centerStructure1Temp = ((Vector2) structure1.TopRightAreaCorner + structure1.BottomLeftAreaCorner) / 2;
        Vector2 centerStructure2Temp = ((Vector2) structure2.TopRightAreaCorner + structure2.BottomLeftAreaCorner) / 2;

        float angle = CalculateAngle(centerStructure1Temp, centerStructure2Temp);

        if ((angle < 45 && angle >= 0) || (angle > -45 && angle < 0))
        {
            return RelativePosition.RIGHT;
        }
        else if (angle > 45 && angle < 135)
        {
            return RelativePosition.UP;
        }
        else if (angle > -135 && angle < -145)
        {
            return RelativePosition.DOWN;
        }
        else
        {
            return RelativePosition.LEFT;
        }
    }

    private float CalculateAngle(Vector2 centerStructure1Temp, Vector2 centerStructure2Temp)
    {
        return Mathf.Atan2(centerStructure2Temp.y - centerStructure1Temp.y, centerStructure2Temp.x - centerStructure1Temp.x) * Mathf.Rad2Deg;
    }
}
