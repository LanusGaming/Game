using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MinimapObjects
{
    [Header("Unexplored Minimap Rooms")]
    public GameObject unexploredTier1;
    public GameObject unexploredTier2H;
    public GameObject unexploredTier2V;
    public GameObject unexploredTier4;

    [Header("Explored Minimap Rooms")]
    public GameObject exploredTier1;
    public GameObject exploredTier2H;
    public GameObject exploredTier2V;
    public GameObject exploredTier4;

    [Header("Corridors")]
    public GameObject corridorTop;
    public GameObject corridorBottom;
    public GameObject corridorLeft;
    public GameObject corridorRight;
}

public class Minimap : MonoBehaviour
{
    public MinimapObjects minimapObjects;
    public Camera minimapCamera;
    public Transform spriteMask;
    public Transform background;
    public Transform minimapRoomsParent;

    private void Start()
    {
        spriteMask.localScale = new Vector3(minimapCamera.orthographicSize * 2, minimapCamera.orthographicSize * 2, 1);
        background.localScale = spriteMask.localScale;
    }

    public void GenerateMinimap(Room[,] level, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (level[x, y] == null)
                    continue;

                Room room = level[x, y];

                if (room.data.location != new Vector2Int(x, y))
                    continue;

                Transform corridorParent;

                if (!room.unexploredMinimapObject)
                    corridorParent = GenerateRoom(room.data);
                else
                {
                    corridorParent = Instantiate(room.unexploredMinimapObject, minimapRoomsParent).transform;
                    corridorParent.localPosition = new Vector2(room.data.location.x, room.data.location.y);
                }

                GenerateCorridors(room.data, corridorParent);

                room.minimapObject = corridorParent;

                if (room.data.isSpawnRoom)
                    minimapRoomsParent.localPosition = new Vector2(room.data.location.x, room.data.location.y) * -1;
                else
                    corridorParent.gameObject.SetActive(false);
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < minimapRoomsParent.childCount; i++)
        {
            Destroy(minimapRoomsParent.GetChild(i).gameObject);
        }
    }

    public Transform ExploreRoom(Room room)
    {
        Destroy(room.minimapObject.gameObject);

        Transform corridorParent;

        if (!room.exploredMinimapObject)
            corridorParent = GenerateRoom(room.data, true);
        else
        {
            corridorParent = Instantiate(room.exploredMinimapObject, minimapRoomsParent).transform;
            corridorParent.localPosition = new Vector2(room.data.location.x, room.data.location.y);
        }

        GenerateCorridors(room.data, corridorParent);

        return corridorParent;
    }

    private Transform GenerateRoom(RoomData room, bool explored = false)
    {
        GameObject roomObject = null;

        switch (room.roomType)
        {
            case RoomType.Type1:
                roomObject = (explored) ? minimapObjects.exploredTier1 : minimapObjects.unexploredTier1;
                break;

            case RoomType.Type2Horizontal:
                roomObject = (explored) ? minimapObjects.exploredTier2H : minimapObjects.unexploredTier2H;
                break;

            case RoomType.Type2Vertical:
                roomObject = (explored) ? minimapObjects.exploredTier2V : minimapObjects.unexploredTier2V;
                break;

            case RoomType.Type4:
                roomObject = (explored) ? minimapObjects.exploredTier4 : minimapObjects.unexploredTier4;
                break;
        }

        GameObject generatedRoom = Instantiate(roomObject, minimapRoomsParent);
        generatedRoom.transform.localPosition = new Vector2(room.location.x, room.location.y);

        return generatedRoom.transform;
    }

    private void GenerateCorridors(RoomData room, Transform parent)
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

            Vector2Int corridorPosition = connectionDirection - RoomData.GetDoorNormal(connectionDirection, room.roomType);
            Instantiate(corridorObject, parent).transform.localPosition = new Vector2(corridorPosition.x, corridorPosition.y);
        }
    }
}