using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MinimapObjects
{
    [Header("Minimap Rooms")]
    public GameObject tier1;
    public GameObject tier2H;
    public GameObject tier2V;
    public GameObject tier4;

    [Header("Corridors")]
    public GameObject corridorTop;
    public GameObject corridorBottom;
    public GameObject corridorLeft;
    public GameObject corridorRight;

    [Header("Boss Room Extensions")]
    public GameObject corner;
    public GameObject wall1H;
    public GameObject wall2H;
    public GameObject wall1V;
    public GameObject wall2V;
}

public class Minimap : MonoBehaviour
{
    public MinimapObjects minimapObjects;
    public Camera minimapCamera;
    public Transform spriteMask;
    public Transform minimapRoomsParent;

    private void Start()
    {
        spriteMask.localScale = new Vector3(minimapCamera.orthographicSize * 2, minimapCamera.orthographicSize * 2, 1);
    }

    // TODO let game controller handle minimap generation
    public void GenerateMinimap(RoomData[] rooms)
    {
        foreach (RoomData room in rooms)
        {
            if (room.isSpawnRoom)
                minimapRoomsParent.localPosition = new Vector2(room.location.x, room .location.y) * -1;

            if (room.isBossRoom)
                GenerateBossRoomExtensions(room);

            GenerateRoom(room);
            GenerateCorridors(room);
        }
    }

    private void GenerateRoom(RoomData room)
    {
        GameObject roomObject = null;

        switch (room.roomType)
        {
            case RoomType.Type1:
                roomObject = minimapObjects.tier1;
                break;

            case RoomType.Type2Horizontal:
                roomObject = minimapObjects.tier2H;
                break;

            case RoomType.Type2Vertical:
                roomObject = minimapObjects.tier2V;
                break;

            case RoomType.Type4:
                roomObject = minimapObjects.tier4;
                break;
        }

        Instantiate(roomObject, minimapRoomsParent).transform.localPosition = new Vector2(room.location.x, room.location.y);
    }

    private void GenerateCorridors(RoomData room)
    {
        foreach (Vector2Int connectionDirection in room.GetDoorConnections())
        {
            GameObject corridorObject = null;

            Vector2Int corridorType = RoomData.GetDoorNormal(connectionDirection, room.roomType);
            
            if (corridorType == Vector2Int.up)
                corridorObject = minimapObjects.corridorTop;
            if (corridorType == Vector2Int.down)
                corridorObject = minimapObjects.corridorBottom;
            if (corridorType == Vector2Int.left)
                corridorObject = minimapObjects.corridorLeft;
            if (corridorType == Vector2Int.right)
                corridorObject = minimapObjects.corridorRight;

            Vector2Int corridorPosition = room.location + connectionDirection - RoomData.GetDoorNormal(connectionDirection, room.roomType);
            Instantiate(corridorObject, minimapRoomsParent).transform.localPosition = new Vector2(corridorPosition.x, corridorPosition.y);
        }
    }

    private void GenerateBossRoomExtensions(RoomData room)
    {
        GameObject horizontalWall = null;
        GameObject verticalWall = null;
        Vector2 offset = Vector2.zero;

        switch (room.roomType)
        {
            case RoomType.Type1:
                horizontalWall = minimapObjects.wall1H;
                verticalWall = minimapObjects.wall1V;
                offset = new Vector2(0.45f, 0.45f);
                break;

            case RoomType.Type2Horizontal:
                horizontalWall = minimapObjects.wall2H;
                verticalWall = minimapObjects.wall1V;
                offset = new Vector2(1.45f, 0.45f);
                break;

            case RoomType.Type2Vertical:
                horizontalWall = minimapObjects.wall1H;
                verticalWall = minimapObjects.wall2V;
                offset = new Vector2(0.45f, 1.45f);
                break;

            case RoomType.Type4:
                horizontalWall = minimapObjects.wall2H;
                verticalWall = minimapObjects.wall2V;
                offset = new Vector2(1.45f, 1.45f);
                break;
        }

        string type = room.roomID.Split("_").Last();

        if (type == "br")
            offset.x *= -1;
        if (type == "tl")
            offset.y *= -1;
        if (type == "tr")
            offset *= -1;

        Instantiate(horizontalWall, minimapRoomsParent).transform.localPosition = new Vector2(room.location.x, room.location.y + offset.y);
        Instantiate(verticalWall, minimapRoomsParent).transform.localPosition = new Vector2(room.location.x + offset.x, room.location.y);
        Instantiate(minimapObjects.corner, minimapRoomsParent).transform.localPosition = new Vector2(room.location.x, room.location.y) + offset;
    }
}