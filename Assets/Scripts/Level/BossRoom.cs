using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BossRoomData
{
    public RoomData topLeft;
    public RoomData topRight;
    public RoomData bottomLeft;
    public RoomData bottomRight;

    public RoomData exitRoom;
    public Vector2Int exitRoomPosition;

    public int GetDoorCount()
    {
        return topLeft.GetDoorCount() + topRight.GetDoorCount() + bottomLeft.GetDoorCount() + bottomRight.GetDoorCount();
    }

    public Dictionary<Vector2Int, Vector2Int> GetDoorConnections()
    {
        Dictionary<Vector2Int, Vector2Int> directions = new Dictionary<Vector2Int, Vector2Int>();

        Vector2Int offset = Vector2Int.zero;

        switch (bottomLeft.roomType)
        {
            case RoomType.Type1:
                offset = new Vector2Int(1, 1);
                break;

            case RoomType.Type2Horizontal:
                offset = new Vector2Int(2, 1);
                break;

            case RoomType.Type2Vertical:
                offset = new Vector2Int(1, 2);
                break;

            case RoomType.Type4:
                offset = new Vector2Int(2, 2);
                break;
        }

        Vector2Int[] connections;

        connections = bottomLeft.GetDoorConnections();
        foreach (Vector2Int connectionDirection in connections)
        {
            directions.Add(connectionDirection, RoomData.GetDoorNormal(connectionDirection, bottomLeft.roomType));
        }

        connections = bottomRight.GetDoorConnections();
        foreach (Vector2Int connectionDirection in connections)
        {
            directions.Add(new Vector2Int(connectionDirection.x + offset.x, connectionDirection.y), RoomData.GetDoorNormal(connectionDirection, bottomRight.roomType));
        }

        connections = topLeft.GetDoorConnections();
        foreach (Vector2Int connectionDirection in connections)
        {
            directions.Add(new Vector2Int(connectionDirection.x, connectionDirection.y + offset.y), RoomData.GetDoorNormal(connectionDirection, topLeft.roomType));
        }

        connections = topRight.GetDoorConnections();
        foreach (Vector2Int connectionDirection in connections)
        {
            directions.Add(connectionDirection + offset, RoomData.GetDoorNormal(connectionDirection, topRight.roomType));
        }

        return directions;
    }

    public Vector2Int GetRoomSize()
    {
        Vector2Int size = Vector2Int.zero;

        switch (bottomLeft.roomType)
        {
            case RoomType.Type1:
                size = new Vector2Int(1, 1);
                break;

            case RoomType.Type2Horizontal:
                size = new Vector2Int(2, 1);
                break;

            case RoomType.Type2Vertical:
                size = new Vector2Int(1, 2);
                break;

            case RoomType.Type4:
                size = new Vector2Int(2, 2);
                break;
        }

        switch (bottomRight.roomType)
        {
            case RoomType.Type1:
                size.x += 1;
                break;

            case RoomType.Type2Horizontal:
                size.x += 2;
                break;

            case RoomType.Type2Vertical:
                size.x += 1;
                break;

            case RoomType.Type4:
                size.x += 2;
                break;
        }

        switch (topLeft.roomType)
        {
            case RoomType.Type1:
                size.y += 1;
                break;

            case RoomType.Type2Horizontal:
                size.y += 1;
                break;

            case RoomType.Type2Vertical:
                size.y += 2;
                break;

            case RoomType.Type4:
                size.y += 2;
                break;
        }

        return size;
    }
}

public class BossRoom : MonoBehaviour
{
    public Room topLeft;
    public Room topRight;
    public Room bottomLeft;
    public Room bottomRight;

    public GameObject exitRoomObject;
    [Tooltip("Relative to the bottom-left corner of the bottom-left tile")]
    public Vector2Int exitRoomPosition;

    public BossRoomData GetBossRoomData()
    {
        BossRoomData bossRoomData = new BossRoomData();

        bossRoomData.topLeft = topLeft.data;
        bossRoomData.topRight = topRight.data;
        bossRoomData.bottomLeft = bottomLeft.data;
        bossRoomData.bottomRight = bottomRight.data;
        bossRoomData.exitRoom = exitRoomObject.GetComponent<Room>().data;
        bossRoomData.exitRoomPosition = exitRoomPosition;

        return bossRoomData;
    }
}
