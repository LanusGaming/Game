using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
    public bool isBossRoom;
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
        isBossRoom = other.isBossRoom;
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
        return isSpawnRoom || isBossRoom || isChestRoom || isHiddenRoom || isLockedRoom;
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

    public Vector2Int[] GetDoorConnections()
    {
        List<Vector2Int> directions = new List<Vector2Int>();

        switch (roomType)
        {
            case RoomType.Type1:
                if (hasDoorTop)
                    directions.Add(new Vector2Int(0, 1));

                if (hasDoorBottom)
                    directions.Add(new Vector2Int(0, -1));

                if (hasDoorLeft)
                    directions.Add(new Vector2Int(-1, 0));

                if (hasDoorRight)
                    directions.Add(new Vector2Int(1, 0));

                break;

            case RoomType.Type2Horizontal:
                if (hasDoorTopLeft)
                    directions.Add(new Vector2Int(0, 1));
                if (hasDoorTopRight)
                    directions.Add(new Vector2Int(1, 1));

                if (hasDoorBottomLeft)
                    directions.Add(new Vector2Int(0, -1));
                if (hasDoorBottomRight)
                    directions.Add(new Vector2Int(1, -1));

                if (hasDoorLeft)
                    directions.Add(new Vector2Int(-1, 0));

                if (hasDoorRight)
                    directions.Add(new Vector2Int(2, 0));

                break;

            case RoomType.Type2Vertical:
                if (hasDoorTop)
                    directions.Add(new Vector2Int(0, 2));

                if (hasDoorBottom)
                    directions.Add(new Vector2Int(0, -1));

                if (hasDoorLeftTop)
                    directions.Add(new Vector2Int(-1, 1));
                if (hasDoorLeftBottom)
                    directions.Add(new Vector2Int(-1, 0));

                if (hasDoorRightTop)
                    directions.Add(new Vector2Int(1, 1));
                if (hasDoorRightBottom)
                    directions.Add(new Vector2Int(1, 0));

                break;

            case RoomType.Type4:
                if (hasDoorTopLeft)
                    directions.Add(new Vector2Int(0, 2));
                if (hasDoorTopRight)
                    directions.Add(new Vector2Int(1, 2));

                if (hasDoorBottomLeft)
                    directions.Add(new Vector2Int(0, -1));
                if (hasDoorBottomRight)
                    directions.Add(new Vector2Int(1, -1));

                if (hasDoorLeftTop)
                    directions.Add(new Vector2Int(-1, 1));
                if (hasDoorLeftBottom)
                    directions.Add(new Vector2Int(-1, 0));

                if (hasDoorRightTop)
                    directions.Add(new Vector2Int(2, 1));
                if (hasDoorRightBottom)
                    directions.Add(new Vector2Int(2, 0));

                break;
        }

        return directions.ToArray();
    }

    public Vector2Int[] GetSurroundingTiles()
    {
        return GetSurroundingTiles(roomType);
    }

    public static Vector2Int GetDoorNormal(Vector2Int direction, RoomType roomType)
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

        if (direction == new Vector2Int(1, 1))
        {
            if (roomType == RoomType.Type2Horizontal)
                return Vector2Int.up;
            if (roomType == RoomType.Type2Vertical)
                return Vector2Int.right;
        }

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
    public Transform room;
    public Trigger[] visibilityTriggers;
    public Trigger[] enteredTriggers;
    public GameObject[] layoutPresets;
    public GameObject unexploredMinimapObject;
    public GameObject exploredMinimapObject;

    [HideInInspector]
    public bool visible = false;
    [HideInInspector]
    public bool cleared = false;
    [HideInInspector]
    public Transform minimapObject;
    [HideInInspector]
    public Room[] connectedRooms;

    private void Start()
    {
        foreach (Trigger trigger in visibilityTriggers)
            trigger.triggeredCallback = OnVisibilityTriggerHit;

        foreach (Trigger trigger in enteredTriggers)
        {
            trigger.triggeredCallback = OnEnteredTriggerHit;
        }
    }

    public void OnVisibilityTriggerHit(Trigger trigger)
    {
        if (visible)
            return;

        if (room)
            room.gameObject.SetActive(true);

        foreach (Room room in connectedRooms)
            if (!room.visible)
                room.minimapObject.gameObject.SetActive(true);

        visible = true;

        foreach (Trigger visibilityTrigger in visibilityTriggers)
            visibilityTrigger.gameObject.SetActive(false);
    }

    public void OnEnteredTriggerHit(Trigger trigger)
    {
        if (cleared)
            return;

        Debug.Log("Entered Combat");

        GameController.instance.minimap.ExploreRoom(this);

        foreach (Trigger enteredTrigger in enteredTriggers)
            enteredTrigger.gameObject.SetActive(false);
    }
}
