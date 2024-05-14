using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    public Room topLeft;
    public Room topRight;
    public Room bottomLeft;
    public Room bottomRight;

    public GameObject exitRoomObject;
    [Tooltip("Relative to the bottom-left corner of the bottom-left tile")]
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

        Dictionary<Vector2Int, Vector2Int> connections;
        
        connections = bottomLeft.GetDoorConnections();
        foreach (Vector2Int connectionDirection in connections.Keys)
        {
            directions.Add(connectionDirection, connections[connectionDirection]);
        }

        connections = bottomRight.GetDoorConnections();
        foreach (Vector2Int connectionDirection in connections.Keys)
        {
            directions.Add(new Vector2Int(connectionDirection.x + offset.x, connectionDirection.y), connections[connectionDirection]);
        }

        connections = bottomLeft.GetDoorConnections();
        foreach (Vector2Int connectionDirection in connections.Keys)
        {
            directions.Add(new Vector2Int(connectionDirection.x, connectionDirection.y + offset.y), connections[connectionDirection]);
        }

        connections = bottomLeft.GetDoorConnections();
        foreach (Vector2Int connectionDirection in connections.Keys)
        {
            directions.Add(connectionDirection + offset, connections[connectionDirection]);
        }

        return directions;
    }
}
