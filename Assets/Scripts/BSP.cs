using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BSP : MonoBehaviour
{
    struct Room
    {
        public Vector3 center;
        public Vector2 extend;


        public List<Room> childrenRooms;
    }
    
    [SerializeField] private int levelSizeX;
    [SerializeField] private int levelSizeY;
    [SerializeField] private int roomSizeX;
    [SerializeField] private int roomSizeY;
    //[SerializeField] private int corridorWidth;

    private Room rootRoom;

    public void Create()
    {
        rootRoom.extend =  new Vector2(levelSizeX * 2, levelSizeY * 2);
        
        rootRoom.center = Vector3.zero;
        
        rootRoom.childrenRooms = new List<Room>();
        
        rootRoom.childrenRooms.AddRange(CheckDivision(rootRoom));
    }

    public void Clear()
    {
        rootRoom = new Room();
    }

    List<Room> CheckDivision(Room room)
    {
        List<Room> childrenList = new List<Room>();

        if (room.extend.x > roomSizeX * 2 && room.extend.y > roomSizeY * 2)
        {
            childrenList.AddRange(ProbabilityDivision(room));
        }
        else if (room.extend.x > roomSizeX * 2)
        {
            childrenList.AddRange(DivisionByX(room));
        }
        else if (room.extend.y > roomSizeY * 2)
        {
            childrenList.AddRange(DivisionByY(room));
        }

        return childrenList;
    }

    List<Room> ProbabilityDivision(Room room)
    {
        float probabilitiy = Random.Range(0f, 1f);

        if (probabilitiy > 0.5f)
        {
            return DivisionByX(room);
        }
        else
        {
            return DivisionByY(room);
        }
    }

    List<Room> DivisionByX(Room room)
    {
        List<Room> rooms = new List<Room>();

        Room leftRoom;
        Room rightRoom;

        float posX = Random.Range(0 + roomSizeX * 0.5f, room.extend.x - roomSizeX * 0.5f);
        
        rightRoom.extend = new Vector2(posX, room.extend.y);
        rightRoom.center = new Vector3(room.center.x + room.extend.x * 0.5f - rightRoom.extend.x * 0.5f, 0, room.center.y);
        rightRoom.childrenRooms = new List<Room>();
        
        leftRoom.extend = new Vector2(room.extend.x - posX, room.extend.y);
        leftRoom.center = new Vector3(room.center.x - room.extend.x * 0.5f + leftRoom.extend.x * 0.5f, 0, room.center.y);
        leftRoom.childrenRooms = new List<Room>();
        
        room.childrenRooms.Add(rightRoom);
        room.childrenRooms.Add(leftRoom);
        
        rightRoom.childrenRooms.AddRange(CheckDivision(rightRoom));
        leftRoom.childrenRooms.AddRange(CheckDivision(leftRoom));

        return rooms;
    }

    List<Room> DivisionByY(Room room)
    {
        List<Room> rooms = new List<Room>();

        Room roomUp;
        Room roomDown;

        float posY = Random.Range(0 + roomSizeY * 0.5f, room.extend.y - roomSizeY * 0.5f);
        
        roomDown.extend = new Vector2(room.extend.x, posY);
        roomDown.center = new Vector3(room.center.x, 0, room.center.y - room.extend.y * 0.5f + roomDown.extend.y * 0.5f);
        roomDown.childrenRooms = new List<Room>();
        
        roomUp.extend = new Vector2(room.extend.x, room.extend.y - posY);
        roomUp.center = new Vector3(room.center.x, 0, room.center.y + room.extend.y * 0.5f - roomUp.extend.y * 0.5f);
        roomUp.childrenRooms = new List<Room>();
        
        room.childrenRooms.Add(roomDown);
        room.childrenRooms.Add(roomUp);
        
        roomDown.childrenRooms.AddRange(CheckDivision(roomDown));
        roomUp.childrenRooms.AddRange(CheckDivision(roomUp));

        return rooms;
    }

    private void OnDrawGizmos()
    {
        DrawRoom(rootRoom);
    }

    static void DrawRoom(Room room)
    {
        Vector3 size = new Vector3(room.extend.x, 0, room.extend.y);
        Gizmos.DrawWireCube(room.center, size);
        
        if (room.childrenRooms == null) return;

        foreach (Room roomChild in room.childrenRooms)
        {
            Gizmos.color = Color.red;
            DrawRoom(roomChild);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BSP))]
public class BSPEditor:Editor {
    public override void OnInspectorGUI() {
        BSP myTarget = (BSP)target;

        if(GUILayout.Button("Generate")) {
            myTarget.Create();
        }

        if(GUILayout.Button("Clear")) {
            myTarget.Clear();
        }

        DrawDefaultInspector();
    }
}
#endif
