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
    
    
    // Start is called before the first frame update
    void Start()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        LevelGenerator generator = new LevelGenerator(levelWidth, levelLength);

        var listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
