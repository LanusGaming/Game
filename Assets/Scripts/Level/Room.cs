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
public class RoomData
{
    [HideInInspector]
    public string roomID;
    [HideInInspector]
    public Vector2Int location;
    [HideInInspector]
    public int distanceFromBossRoom;

    [Header("Type Info")]
    public RoomType roomType;

    public bool isSpawnRoom;
    public bool isChestRoom;
    public bool isHiddenRoom;
    public bool isLockedRoom;

    [Header("Top Doors")]
    public bool hasDoorTopLeft;
    public bool hasDoorTop;
    public bool hasDoorTopRight;

    [Header("Bottom Doors")]
    public bool hasDoorBottomLeft;
    public bool hasDoorBottom;
    public bool hasDoorBottomRight;

    [Header("Left Doors")]
    public bool hasDoorLeftTop;
    public bool hasDoorLeft;
    public bool hasDoorLeftBottom;

    [Header("Right Doors")]
    public bool hasDoorRightTop;
    public bool hasDoorRight;
    public bool hasDoorRightBottom;

    public RoomData() { }

    public RoomData(RoomData other)
    {
        roomID = other.roomID;
        location = new Vector2Int();
        distanceFromBossRoom = -1;

        roomType = other.roomType;

        isSpawnRoom = other.isSpawnRoom;
        isChestRoom = other.isChestRoom;
        isHiddenRoom = other.isHiddenRoom;
        isLockedRoom = other.isLockedRoom;

        hasDoorTopLeft = other.hasDoorTopLeft;
        hasDoorTop = other.hasDoorTop;
        hasDoorTopRight = other.hasDoorTopRight;

        hasDoorBottomLeft = other.hasDoorBottomLeft;
        hasDoorBottom = other.hasDoorBottom;
        hasDoorBottomRight = other.hasDoorBottomRight;

        hasDoorLeftTop = other.hasDoorLeftTop;
        hasDoorLeft = other.hasDoorLeft;
        hasDoorLeftBottom = other.hasDoorLeftBottom;

        hasDoorRightTop = other.hasDoorRightTop;
        hasDoorRight = other.hasDoorRight;
        hasDoorRightBottom = other.hasDoorRightBottom;
    }

    public bool IsEquivalentTo(RoomData other)
    {
        if (roomType != other.roomType)
            return false;

        if (hasDoorTopLeft == other.hasDoorTopLeft && hasDoorTop == other.hasDoorTop && hasDoorTopRight == other.hasDoorTopRight
            && hasDoorBottomLeft == other.hasDoorBottomLeft && hasDoorBottom == other.hasDoorBottom && hasDoorBottomRight == other.hasDoorBottomRight
            && hasDoorLeftTop == other.hasDoorLeftTop && hasDoorLeft == other.hasDoorLeft && hasDoorLeftBottom == other.hasDoorLeftBottom
            && hasDoorRightTop == other.hasDoorRightTop && hasDoorRight == other.hasDoorRight && hasDoorRightBottom == other.hasDoorRightBottom)
            return true;

        return false;
    }

    public bool IsSpecialRoom()
    {
        return isSpawnRoom || isChestRoom || isHiddenRoom || isLockedRoom;
    }

    public int GetDoorCount()
    {
        int count = 0;

        if (hasDoorTopLeft)
            count++;
        if (hasDoorTop)
            count++;
        if (hasDoorTopRight)
            count++;

        if (hasDoorBottomLeft)
            count++;
        if (hasDoorBottom)
            count++;
        if (hasDoorBottomRight)
            count++;

        if (hasDoorLeftTop)
            count++;
        if (hasDoorLeft)
            count++;
        if (hasDoorLeftBottom)
            count++;

        if (hasDoorRightTop)
            count++;
        if (hasDoorRight)
            count++;
        if (hasDoorRightBottom)
            count++;

        return count;
    }

    public Dictionary<Vector2Int, Vector2Int> GetDoorConnections()
    {
        Dictionary<Vector2Int, Vector2Int> directions = new Dictionary<Vector2Int, Vector2Int>();

        switch (roomType)
        {
            case RoomType.Type1:
                if (hasDoorTop)
                    directions.Add(new Vector2Int(0, 1), Vector2Int.up);

                if (hasDoorBottom)
                    directions.Add(new Vector2Int(0, -1), Vector2Int.down);

                if (hasDoorLeft)
                    directions.Add(new Vector2Int(-1, 0), Vector2Int.left);

                if (hasDoorRight)
                    directions.Add(new Vector2Int(1, 0), Vector2Int.right);

                break;

            case RoomType.Type2Horizontal:
                if (hasDoorTopLeft)
                    directions.Add(new Vector2Int(0, 1), Vector2Int.up);
                if (hasDoorTopRight)
                    directions.Add(new Vector2Int(1, 1), Vector2Int.up);

                if (hasDoorBottomLeft)
                    directions.Add(new Vector2Int(0, -1), Vector2Int.down);
                if (hasDoorBottomRight)
                    directions.Add(new Vector2Int(1, -1), Vector2Int.down);

                if (hasDoorLeft)
                    directions.Add(new Vector2Int(-1, 0), Vector2Int.left);

                if (hasDoorRight)
                    directions.Add(new Vector2Int(2, 0), Vector2Int.right);

                break;

            case RoomType.Type2Vertical:
                if (hasDoorTop)
                    directions.Add(new Vector2Int(0, 2), Vector2Int.up);

                if (hasDoorBottom)
                    directions.Add(new Vector2Int(0, -1), Vector2Int.down);

                if (hasDoorLeftTop)
                    directions.Add(new Vector2Int(-1, 1), Vector2Int.left);
                if (hasDoorLeftBottom)
                    directions.Add(new Vector2Int(-1, 0), Vector2Int.left);

                if (hasDoorRightTop)
                    directions.Add(new Vector2Int(1, 1), Vector2Int.right);
                if (hasDoorRightBottom)
                    directions.Add(new Vector2Int(1, 0), Vector2Int.right);

                break;

            case RoomType.Type4:
                if (hasDoorTopLeft)
                    directions.Add(new Vector2Int(0, 2), Vector2Int.up);
                if (hasDoorTopRight)
                    directions.Add(new Vector2Int(1, 2), Vector2Int.up);

                if (hasDoorBottomLeft)
                    directions.Add(new Vector2Int(0, -1), Vector2Int.down);
                if (hasDoorBottomRight)
                    directions.Add(new Vector2Int(1, -1), Vector2Int.down);

                if (hasDoorLeftTop)
                    directions.Add(new Vector2Int(-1, 1), Vector2Int.left);
                if (hasDoorLeftBottom)
                    directions.Add(new Vector2Int(-1, 0), Vector2Int.left);

                if (hasDoorRightTop)
                    directions.Add(new Vector2Int(2, 1), Vector2Int.right);
                if (hasDoorRightBottom)
                    directions.Add(new Vector2Int(2, 0), Vector2Int.right);

                break;
        }

        return directions;
    }

    public Vector2Int[] GetSurroundingTiles()
    {
        return GetSurroundingTiles(roomType);
    }

    public static Vector2Int GetDoorNormal(Vector2Int direction)
    {
        if (direction == new Vector2Int(0, 1))
            return Vector2Int.up;
        if (direction == new Vector2Int(0, -1))
            return Vector2Int.down;
        if (direction == new Vector2Int(1, 0))
            return Vector2Int.right;
        if (direction == new Vector2Int(-1, 0))
            return Vector2Int.left;
        if (direction == new Vector2Int(0, 2))
            return Vector2Int.up;
        if (direction == new Vector2Int(1, 2))
            return Vector2Int.up;
        if (direction == new Vector2Int(1, -1))
            return Vector2Int.down;
        if (direction == new Vector2Int(2, 1))
            return Vector2Int.right;
        if (direction == new Vector2Int(2, 0))
            return Vector2Int.right;
        if (direction == new Vector2Int(-1, 1))
            return Vector2Int.left;

        return Vector2Int.zero;
    }

    public static Vector2Int[] GetSurroundingTiles(RoomType roomType)
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

public class Room : MonoBehaviour
{
    public RoomData data;
    public GameObject[] layoutPresets;
    public Trigger[] triggers;
}
