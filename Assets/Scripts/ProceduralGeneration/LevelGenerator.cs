using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator
{
    List<RoomNode> allNodesCollection= new List<RoomNode>();
    private int levelWidth;
    private int levelLength;

    public LevelGenerator(int levelWidth, int levelLength)
    {
        this.levelWidth = levelWidth;
        this.levelLength = levelLength;
    }

    public List<Node> CalculateLevel(int maxIterations, int roomWidthMin, int roomLengthMin, int corridorWidth)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(levelWidth, levelLength);
        allNodesCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeaf(bsp.RootNode);
        
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomInGivenSpace(roomSpaces);
        
        CorridorGenerator corridorGenerator = new CorridorGenerator();
        List<Node> corridors = corridorGenerator.CreateCorridor(allNodesCollection, corridorWidth);
        
        return new List<Node>(roomList).Concat(corridors).ToList();
    }
}
