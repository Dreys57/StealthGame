using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [SerializeField] private int levelWidth;
    [SerializeField] private int levelLength;
    [SerializeField] private int roomWidthMin;
    [SerializeField] private int roomLengthMin;
    [SerializeField] private int maxIterations;
    [SerializeField] private int corridorWidth;

    [SerializeField] private Material material;
    
    
    void Start()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        LevelGenerator generator = new LevelGenerator(levelWidth, levelLength);

        var listOfRooms = generator.CalculateLevel(maxIterations, roomWidthMin, roomLengthMin, corridorWidth);

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
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
        
        Vector2[]uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        
        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };
        
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        
        GameObject levelFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));
        
        levelFloor.transform.position = Vector3.zero;
        levelFloor.transform.localScale = Vector3.one;
        levelFloor.GetComponent<MeshFilter>().mesh = mesh;
        levelFloor.GetComponent<MeshRenderer>().material = material;
    }
}
