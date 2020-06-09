using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelCreator : MonoBehaviour
{
    [SerializeField] private int levelWidth;
    [SerializeField] private int levelLength;
    [SerializeField] private int roomWidthMin;
    [SerializeField] private int roomLengthMin;
    [SerializeField] private int maxIterations;
    [SerializeField] private int corridorWidth;
    [Range(0, 2)]
    [SerializeField] private int offset;
    
    [Range(0.0f, 0.3f)]
    [SerializeField] private float bottomLeftPointModifier;
    [Range(0.7f, 1.0f)]
    [SerializeField] private float topRightPointModifier;

    [SerializeField] private Material material;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject goalPrefab;

    private List<Vector3Int> verticalDoorPossiblePositions;
    private List<Vector3Int> horizontalDoorPossiblePositions;
    private List<Vector3Int> horizontalWallPossiblePositions;
    private List<Vector3Int> verticalWallPossiblePositions;

    private bool foundStartRoom;
    private bool foundEndRoom;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] AStar;
    [SerializeField] private GameObject[] guards;

    private int maxAStarIndex = 1;
    private int AStarIndex = 0;
    private int guardsIndex = 0;


    private void Awake()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        LevelGenerator generator = new LevelGenerator(levelWidth, levelLength);

        List<Node> listOfRooms = generator.CalculateLevel(
            maxIterations, 
            roomWidthMin, 
            roomLengthMin, 
            corridorWidth, 
            bottomLeftPointModifier, 
            topRightPointModifier,
            offset);
        
        GameObject parentWall = new GameObject("ParentWall");
        parentWall.transform.parent = transform;
        
        verticalDoorPossiblePositions = new List<Vector3Int>();
        horizontalDoorPossiblePositions = new List<Vector3Int>();
        
        verticalWallPossiblePositions = new List<Vector3Int>();
        horizontalWallPossiblePositions = new List<Vector3Int>();
        
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }

        CreateWalls(parentWall);
        
        SpawnObjects(listOfRooms);
        
        SpawnPlayerAndGuards(listOfRooms);
        
        for (int i = 0; i < AStar.Length; i++)
        {
            AStar[i].GetComponentInChildren<Grid>().FinishedLevelGeneration1 = true;
        }
    }

    void SpawnObjects(List<Node> rooms)
    {
        foreach (Node room in rooms)
        {
            for (int i = 0; i < 5; i++)
            {
                if (room.Parent != null)
                {
                    Vector3 spawnObstaclePos = new Vector3(
                        Random.Range(room.BottomLeftAreaCorner.x, room.TopRightAreaCorner.x),
                        0f,
                        Random.Range(room.BottomLeftAreaCorner.y, room.TopRightAreaCorner.y));
                    
                    Instantiate(obstaclePrefab, spawnObstaclePos, Quaternion.identity);
                }
            }
        }
    }

    void SpawnPlayerAndGuards(List<Node> rooms)
    {
        foreach (Node room in rooms)
        {
            if (room.Parent != null)
            {
                if (!foundStartRoom)
                {
                    Vector3 spawnPlayerPos = new Vector3(
                        Random.Range(room.BottomLeftAreaCorner.x, room.TopRightAreaCorner.x),
                        2f,
                        Random.Range(room.BottomLeftAreaCorner.y, room.TopRightAreaCorner.y));

                    player.transform.position = spawnPlayerPos;

                    foundStartRoom = true;
                    
                    continue;
                }
                
               if(!foundEndRoom)
                {
                    Vector3 sum = new Vector3(room.BottomLeftAreaCorner.x, 2f, room.BottomLeftAreaCorner.y) 
                                  + new Vector3(room.TopRightAreaCorner.x, 2f, room.TopRightAreaCorner.y);

                    Vector3 center = sum / 2;

                    Instantiate(goalPrefab, center, Quaternion.identity);

                    foundEndRoom = true;
                    
                    continue;
                }

               int randomValue = Random.Range(0, 2);

                if (randomValue > 0.5f)
                {
                    if (AStarIndex < maxAStarIndex)
                    {
                        Vector3 sum = new Vector3(room.BottomLeftAreaCorner.x, 2f, room.BottomLeftAreaCorner.y) 
                                      + new Vector3(room.TopRightAreaCorner.x, 2f, room.TopRightAreaCorner.y);

                        Vector3 center = sum / 2;

                        AStar[AStarIndex].transform.position = center;
                
                        AStar[AStarIndex].GetComponent<Grid>().GridWorldSize = new Vector2(
                            room.TopRightAreaCorner.x - room.BottomLeftAreaCorner.x, 
                            room.TopRightAreaCorner.y - room.BottomLeftAreaCorner.y);

                        AStarIndex++;
                        
                        guards[guardsIndex].transform.position = center;

                        guardsIndex ++;
                    }
                } 
            }
        }

        for (int i = 0; i < guards.Length; i++)
        {
            guards[i].SetActive(true);
        }
    }
    
    private void CreateWalls(GameObject parentWall)
    {
        foreach (Vector3Int wallPosition in horizontalWallPossiblePositions)
        {
            Instantiate(wallPrefab, wallPosition, Quaternion.identity, parentWall.transform);
        }

        foreach (Vector3Int wallPosition in verticalWallPossiblePositions)
        {
            Instantiate(wallPrefab, wallPosition, Quaternion.Euler(0, 90, 0), parentWall.transform);
        }
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        //Vertices
        Vector3 bottomLeftVertice = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightVertice = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftVertice = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightVertice = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftVertice,
            topRightVertice,
            bottomLeftVertice,
            bottomRightVertice
        };
        
        //Uvs
        Vector2[]uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        
        //Triangles
        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };

        //Positionning the walls
        
        //On the X axis
        for (int row = (int) bottomLeftVertice.x ; row < (int) bottomRightVertice.x; row++)
        {
            Vector3 wallPosition = new Vector3(row, 0, bottomLeftVertice.z);

            AddWallPositionToList(wallPosition, horizontalWallPossiblePositions, horizontalDoorPossiblePositions);
        }

        for (int row = (int) topLeftVertice.x; row < (int) topRightCorner.x; row++)
        {
            Vector3 wallPosition = new Vector3(row, 0, topRightVertice.z);
            
            AddWallPositionToList(wallPosition, horizontalWallPossiblePositions, horizontalDoorPossiblePositions);
        }
        
        //On the Z axis
        for (int columns = (int) bottomLeftVertice.z; columns < (int) topLeftVertice.z; columns++)
        {
            Vector3 wallPosition = new Vector3(bottomLeftVertice.x, 0, columns);
            
            AddWallPositionToList(wallPosition, verticalWallPossiblePositions, verticalDoorPossiblePositions);
        }
        
        for (int columns = (int) bottomRightVertice.z; columns < (int) topRightVertice.z; columns++)
        {
            Vector3 wallPosition = new Vector3(bottomRightVertice.x, 0, columns);
            
            AddWallPositionToList(wallPosition, verticalWallPossiblePositions, verticalDoorPossiblePositions);
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);

        if (wallList.Contains(point))
        {
            doorList.Add(point);

            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }
}
