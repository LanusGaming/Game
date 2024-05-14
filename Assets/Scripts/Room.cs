using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[Serializable]
public enum RoomType
{
    Type1, Type2Horizontal, Type2Vertical, Type4
}

[Serializable]
public struct Doors
{
    [Header("Top")]
    public bool hasDoorTopLeft;
    public bool hasDoorTop;
    public bool hasDoorTopRight;

    [Header("Bottom")]
    public bool hasDoorBottomLeft;
    public bool hasDoorBottom;
    public bool hasDoorBottomRight;

    [Header("Left")]
    public bool hasDoorLeftTop;
    public bool hasDoorLeft;
    public bool hasDoorLeftBottom;

    [Header("Right")]
    public bool hasDoorRightTop;
    public bool hasDoorRight;
    public bool hasDoorRightBottom;

    public static bool operator ==(Doors a, Doors b)
    {
        if (a.hasDoorTopLeft == b.hasDoorTopLeft && a.hasDoorTop == b.hasDoorTop && a.hasDoorTopRight == b.hasDoorTopRight
            && a.hasDoorBottomLeft == b.hasDoorBottomLeft && a.hasDoorBottom == b.hasDoorBottom && a.hasDoorBottomRight == b.hasDoorBottomRight
            && a.hasDoorLeftTop == b.hasDoorLeftTop && a.hasDoorLeft == b.hasDoorLeft && a.hasDoorLeftBottom == b.hasDoorLeftBottom
            && a.hasDoorRightTop == b.hasDoorRightTop && a.hasDoorRight == b.hasDoorRight && a.hasDoorRightBottom == b.hasDoorRightBottom)
            return true;

        return false;
    }

    public static bool operator !=(Doors a, Doors b) => !(a == b);
}

public class Room : MonoBehaviour
{
    [Header("Room Info")]
    public RoomType roomType;

    public bool isSpawnRoom;
    public bool isChestRoom;
    public bool isHiddenRoom;
    public bool isLockedRoom;
    
    public Doors doors;

    public GameObject[] layoutPresets;

    [Header("Other")]
    public LevelTrigger[] triggers;

    [HideInInspector]
    public Vector2Int location;
    [HideInInspector]
    public int distanceFromBossRoom = -1;

    public bool IsSpecialRoom()
    {
        return isSpawnRoom || isChestRoom || isHiddenRoom || isLockedRoom;
    }

    public int GetDoorCount()
    {
        int count = 0;

        if (doors.hasDoorTopLeft)
            count++;
        if (doors.hasDoorTop)
            count++;
        if (doors.hasDoorTopRight)
            count++;

        if (doors.hasDoorBottomLeft)
            count++;
        if (doors.hasDoorBottom)
            count++;
        if (doors.hasDoorBottomRight)
            count++;

        if (doors.hasDoorLeftTop)
            count++;
        if (doors.hasDoorLeft)
            count++;
        if (doors.hasDoorLeftBottom)
            count++;

        if (doors.hasDoorRightTop)
            count++;
        if (doors.hasDoorRight)
            count++;
        if (doors.hasDoorRightBottom)
            count++;
        
        return count;
    }

    public Dictionary<Vector2Int, Vector2Int> GetDoorConnections()
    {
        Dictionary<Vector2Int, Vector2Int> directions = new Dictionary<Vector2Int, Vector2Int>();

        switch (roomType)
        {
            case RoomType.Type1:
                if (doors.hasDoorTop)
                    directions.Add(new Vector2Int(0, 1), Vector2Int.up);

                if (doors.hasDoorBottom)
                    directions.Add(new Vector2Int(0, -1), Vector2Int.down);

                if (doors.hasDoorLeft)
                    directions.Add(new Vector2Int(-1, 0), Vector2Int.left);

                if (doors.hasDoorRight)
                    directions.Add(new Vector2Int(1, 0), Vector2Int.right);

                break;

            case RoomType.Type2Horizontal:
                if (doors.hasDoorTopLeft)
                    directions.Add(new Vector2Int(0, 1), Vector2Int.up);
                if (doors.hasDoorTopRight)
                    directions.Add(new Vector2Int(1, 1), Vector2Int.up);

                if (doors.hasDoorBottomLeft)
                    directions.Add(new Vector2Int(0, -1), Vector2Int.down);
                if (doors.hasDoorBottomRight)
                    directions.Add(new Vector2Int(1, -1), Vector2Int.down);

                if (doors.hasDoorLeft)
                    directions.Add(new Vector2Int(-1, 0), Vector2Int.left);

                if (doors.hasDoorRight)
                    directions.Add(new Vector2Int(2, 0), Vector2Int.right);

                break;

            case RoomType.Type2Vertical:
                if (doors.hasDoorTop)
                    directions.Add(new Vector2Int(0, 2), Vector2Int.up);

                if (doors.hasDoorBottom)
                    directions.Add(new Vector2Int(0, -1), Vector2Int.down);

                if (doors.hasDoorLeftTop)
                    directions.Add(new Vector2Int(-1, 1), Vector2Int.left);
                if (doors.hasDoorLeftBottom)
                    directions.Add(new Vector2Int(-1, 0), Vector2Int.left);

                if (doors.hasDoorRightTop)
                    directions.Add(new Vector2Int(1, 1), Vector2Int.right);
                if (doors.hasDoorRightBottom)
                    directions.Add(new Vector2Int(1, 0), Vector2Int.right);

                break;

            case RoomType.Type4:
                if (doors.hasDoorTopLeft)
                    directions.Add(new Vector2Int(0, 2), Vector2Int.up);
                if (doors.hasDoorTopRight)
                    directions.Add(new Vector2Int(1, 2), Vector2Int.up);

                if (doors.hasDoorBottomLeft)
                    directions.Add(new Vector2Int(0, -1), Vector2Int.down);
                if (doors.hasDoorBottomRight)
                    directions.Add(new Vector2Int(1, -1), Vector2Int.down);

                if (doors.hasDoorLeftTop)
                    directions.Add(new Vector2Int(-1, 1), Vector2Int.left);
                if (doors.hasDoorLeftBottom)
                    directions.Add(new Vector2Int(-1, 0), Vector2Int.left);

                if (doors.hasDoorRightTop)
                    directions.Add(new Vector2Int(2, 1), Vector2Int.right);
                if (doors.hasDoorRightBottom)
                    directions.Add(new Vector2Int(2, 0), Vector2Int.right);

                break;
        }

        return directions;
    }

    public static Vector2Int[] GetSurroundings(RoomType roomType)
    {
        List<Vector2Int> directions = new List<Vector2Int>();

        switch (roomType)
        {
            case RoomType.Type1:
                directions.Add(new Vector2Int(0, 1));
                directions.Add(new Vector2Int(0, -1));
                directions.Add(new Vector2Int(-1, 0));
                directions.Add(new Vector2Int(1, 0));

                break;

            case RoomType.Type2Horizontal:
                directions.Add(new Vector2Int(0, 1));
                directions.Add(new Vector2Int(1, 1));
                directions.Add(new Vector2Int(0, -1));
                directions.Add(new Vector2Int(1, -1));
                directions.Add(new Vector2Int(-1, 0));
                directions.Add(new Vector2Int(2, 0));

                break;

            case RoomType.Type2Vertical:
                directions.Add(new Vector2Int(0, 2));
                directions.Add(new Vector2Int(0, -1));
                directions.Add(new Vector2Int(-1, 1));
                directions.Add(new Vector2Int(-1, 0));
                directions.Add(new Vector2Int(1, 1));
                directions.Add(new Vector2Int(1, 0));

                break;

            case RoomType.Type4:
                directions.Add(new Vector2Int(0, 2));
                directions.Add(new Vector2Int(1, 2));
                directions.Add(new Vector2Int(0, -1));
                directions.Add(new Vector2Int(1, -1));
                directions.Add(new Vector2Int(-1, 1));
                directions.Add(new Vector2Int(-1, 0));
                directions.Add(new Vector2Int(2, 1));
                directions.Add(new Vector2Int(2, 0));

                break;
        }

        return directions.ToArray();
    }
}
