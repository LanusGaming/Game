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
    public Camera mapCamera;
    public Transform playerRepresantation;
    public Transform minimapSpriteMask;
    public Transform minimapBackground;
    public Transform mapSpriteMask;
    public Transform mapBackground;
    public Transform roomsParent;
    public float mapZoomSensitivity = 0.2f;
    public float minZoomSize = 1f;

    [HideInInspector]
    public bool inMinimapMode = true;

    private Vector3 mousePoint = Vector3.zero;
    private float originalCameraSize;
    private Coroutine changingRoomRoutine;

    private void Start()
    {
        minimapSpriteMask.localScale = new Vector3(minimapCamera.orthographicSize * 2, minimapCamera.orthographicSize * 2, 1);
        minimapBackground.localScale = minimapSpriteMask.localScale;

        if (mapCamera != null)
        {
            mapSpriteMask.localScale = new Vector3(mapCamera.orthographicSize * 2f * Screen.width / Screen.height, mapCamera.orthographicSize * 2f, 1);
            mapBackground.localScale = mapSpriteMask.localScale;
        }
    }

    private void Update()
    {
        if (!inMinimapMode)
        {
            if (mousePoint != Vector3.zero)
            {
                Vector3 point = mapCamera.ScreenToWorldPoint(Input.mousePosition);
                mapCamera.transform.localPosition += mousePoint - new Vector3(point.x, point.y);
            }

            if (Input.GetMouseButtonDown(0))
            {
                mousePoint = mapCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePoint = new Vector3(mousePoint.x, mousePoint.y);
            }

            if (Input.GetMouseButtonUp(0))
            {
                mousePoint = Vector3.zero;
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (changingRoomRoutine != null)
                    StopCoroutine(changingRoomRoutine);

                changingRoomRoutine = StartCoroutine(ChangeRoom(mapCamera.ScreenToWorldPoint(Input.mousePosition)));
            }

            if (Input.mouseScrollDelta != Vector2.zero)
            {
                mapCamera.orthographicSize += Input.mouseScrollDelta.y * mapZoomSensitivity;
                mapCamera.orthographicSize = Mathf.Max(minZoomSize, mapCamera.orthographicSize);

                mapBackground.localScale = new Vector3(mapCamera.orthographicSize * 2f * Screen.width / Screen.height, mapCamera.orthographicSize * 2f, 1);
            }
        }
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
                    corridorParent = Instantiate(room.unexploredMinimapObject, roomsParent).transform;
                    corridorParent.localPosition = new Vector2(room.data.location.x, room.data.location.y);
                }

                GenerateCorridors(room.data, corridorParent);

                room.minimapObject = corridorParent;

                if (room.data.isSpawnRoom)
                    roomsParent.localPosition = new Vector2(room.data.location.x, room.data.location.y) * -1;
                else
                    corridorParent.gameObject.SetActive(false);
            }
        }
    }

    public void Clear()
    {
        GameObject[] children = new GameObject[roomsParent.childCount];

        for (int i = 0; i < roomsParent.childCount; i++)
        {
            children[i] = roomsParent.GetChild(i).gameObject;
        }

        foreach (GameObject child in children)
        {
            child.SetActive(false);
            Destroy(child);
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
            corridorParent = Instantiate(room.exploredMinimapObject, roomsParent).transform;
            corridorParent.localPosition = new Vector2(room.data.location.x, room.data.location.y);
        }

        GenerateCorridors(room.data, corridorParent);

        return corridorParent;
    }

    public bool SwitchMode()
    {
        if (mapCamera == null)
            return false;

        if (changingRoomRoutine != null)
            return false;

        inMinimapMode = !inMinimapMode;

        if (inMinimapMode)
        {
            mapCamera.gameObject.SetActive(false);
            minimapCamera.gameObject.SetActive(true);

            mapCamera.transform.localPosition = new Vector3(0f, 0f, -1f);
            mapCamera.orthographicSize = originalCameraSize;
            mapBackground.localScale = new Vector3(mapCamera.orthographicSize * 2f * Screen.width / Screen.height, mapCamera.orthographicSize * 2f, 1);
        }
        else
        {
            minimapCamera.gameObject.SetActive(false);
            mapCamera.gameObject.SetActive(true);

            originalCameraSize = mapCamera.orthographicSize;
        }

        return true;
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

        GameObject generatedRoom = Instantiate(roomObject, roomsParent);
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

    private IEnumerator ChangeRoom(Vector2 newPosition)
    {
        Debug.Log("Changing Room!");
        yield return new WaitForSeconds(2f);
        changingRoomRoutine = null;
    }
}