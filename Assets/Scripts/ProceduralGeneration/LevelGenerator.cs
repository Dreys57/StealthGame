using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator
{
    List<RoomNode> allSpaceNodes= new List<RoomNode>();
    private int levelWidth;
    private int levelLength;

    public LevelGenerator(int levelWidth, int levelLength)
    {
        this.levelWidth = levelWidth;
        this.levelLength = levelLength;
    }

    public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(levelWidth, levelLength);

        allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);

        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeaf(bsp.RootNode);
        
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);

        List<RoomNode> roomList = roomGenerator.GenerateRoomInGivenSpace(roomSpaces);

        return new List<Node>(roomList);
    }
}
