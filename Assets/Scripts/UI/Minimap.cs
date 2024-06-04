using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

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
    public float quickTravelSpeed = 3f;
    public bool showQuickTravelPath = true;
    public float quickTravelPathWidth = 0.05f;
    public Color quickTravelPathColor = Color.green;
    public Material quickTravelPathMaterial;

    [HideInInspector]
    public bool inMapMode = false;

    private Player player;

    private Vector2Int roomSizeInTiles;
    private Vector3 mousePoint = Vector3.zero;
    private float originalCameraSize;
    private Coroutine changingRoomRoutine;
    private SpriteRenderer destinationSpriteRenderer;
    private Color destinationSpritePreviousColor;
    private Dictionary<Room, int> distanceValues;

    private void Start()
    {
        player = Player.Instance;

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
        if (inMapMode && InputManager.Pressed(Settings.Controls.Exit, "map"))
            SwitchMode();

        if (InputManager.Pressed(Settings.Controls.OpenMap, "map") && mapCamera != null && (!inMapMode && player.active || inMapMode && !player.active))
            SwitchMode();

        if (!inMapMode && playerRepresantation != null)
        {
            playerRepresantation.localPosition = new Vector3(player.transform.position.x / roomSizeInTiles.x, player.transform.position.y / roomSizeInTiles.y);
        }
        else if (inMapMode)
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
                {
                    StopCoroutine(changingRoomRoutine);

                    if (destinationSpriteRenderer != null)
                        destinationSpriteRenderer.color = destinationSpritePreviousColor;
                }

                changingRoomRoutine = StartCoroutine(ChangeRoom(mapCamera.ScreenToWorldPoint(Input.mousePosition)));
            }

            if (Input.mouseScrollDelta != Vector2.zero)
            {
                mapCamera.orthographicSize += -Input.mouseScrollDelta.y * mapZoomSensitivity * mapCamera.orthographicSize / 5f;
                mapCamera.orthographicSize = Mathf.Max(minZoomSize, mapCamera.orthographicSize);

                mapSpriteMask.localScale = new Vector3(mapCamera.orthographicSize * 2f * Screen.width / Screen.height, mapCamera.orthographicSize * 2f, 1);
                mapBackground.localScale = mapSpriteMask.localScale;
            }
        }
    }

    public void GenerateMinimap(Room[,] level, Vector2Int size, Vector2Int roomSizeInTiles)
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


                if (room.data.isBossRoom)
                {
                    BossRoom bossRoom = room.gameObject.GetComponent<BossRoom>();
                    GenerateCorridors(bossRoom.GetBossRoomData(), corridorParent);
                }
                else
                    GenerateCorridors(room.data, corridorParent);

                room.minimapObject = corridorParent;

                if (room.data.isSpawnRoom)
                    roomsParent.localPosition = new Vector2(room.data.location.x, room.data.location.y) * -1;
                else
                    corridorParent.gameObject.SetActive(false);
            }
        }

        this.roomSizeInTiles = roomSizeInTiles;
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

        if (room.data.isBossRoom)
        {
            BossRoom bossRoom = room.gameObject.GetComponent<BossRoom>();
            GenerateCorridors(bossRoom.GetBossRoomData(), corridorParent);
        }
        else
            GenerateCorridors(room.data, corridorParent);

        return corridorParent;
    }

    private void SwitchMode()
    {
        if (changingRoomRoutine != null)
            return;

        inMapMode = !inMapMode;

        if (inMapMode)
        {
            InputManager.Focus("map");

            minimapCamera.gameObject.SetActive(false);
            mapCamera.gameObject.SetActive(true);

            mapCamera.transform.localPosition = new Vector3(playerRepresantation.localPosition.x, playerRepresantation.localPosition.y, -1f);
            originalCameraSize = mapCamera.orthographicSize;

            player.active = false;
        }
        else
        {
            InputManager.Unfocus();

            mapCamera.gameObject.SetActive(false);
            minimapCamera.gameObject.SetActive(true);

            mapCamera.orthographicSize = originalCameraSize;

            mapSpriteMask.localScale = new Vector3(mapCamera.orthographicSize * 2f * Screen.width / Screen.height, mapCamera.orthographicSize * 2f, 1);
            mapBackground.localScale = mapSpriteMask.localScale;

            player.active = true;
        }
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

    private void GenerateCorridors(BossRoomData bossRoom, Transform parent)
    {
        Dictionary<Vector2Int, Vector2Int> connections = bossRoom.GetDoorConnections();
        foreach (Vector2Int connectionDirection in connections.Keys)
        {
            GameObject corridorObject = null;

            Vector2Int corridorType = connections[connectionDirection];

            if (corridorType == Vector2Int.up)
                corridorObject = minimapObjects.corridorTop;
            if (corridorType == Vector2Int.down)
                corridorObject = minimapObjects.corridorBottom;
            if (corridorType == Vector2Int.left)
                corridorObject = minimapObjects.corridorLeft;
            if (corridorType == Vector2Int.right)
                corridorObject = minimapObjects.corridorRight;

            Vector2Int corridorPosition = connectionDirection - connections[connectionDirection];
            Instantiate(corridorObject, parent).transform.localPosition = new Vector2(corridorPosition.x, corridorPosition.y);
        }
    }

    private IEnumerator ChangeRoom(Vector2 newPosition)
    {
        Debug.Log("Changing Room!");

        Vector2Int offset = - new Vector2Int((int)roomsParent.transform.localPosition.x, (int)roomsParent.transform.localPosition.y);
        Vector2Int currentRoomPosition = new Vector2Int(Mathf.RoundToInt(playerRepresantation.localPosition.x), Mathf.RoundToInt(playerRepresantation.localPosition.y)) + offset;
        Vector2Int destinationRoomPosition = new Vector2Int(Mathf.RoundToInt(newPosition.x), Mathf.RoundToInt(newPosition.y)) + offset;

        Room currentRoom = GameController.Instance.level[currentRoomPosition.x, currentRoomPosition.y];

        if (destinationRoomPosition.x < 0 || destinationRoomPosition.x >= GameController.Instance.levelSize.x || destinationRoomPosition.y < 0 || destinationRoomPosition.y >= GameController.Instance.levelSize.y)
        {
            changingRoomRoutine = null;
            yield break;
        }

        Room destinationRoom = GameController.Instance.level[destinationRoomPosition.x, destinationRoomPosition.y];

        if (destinationRoom == currentRoom)
        {
            changingRoomRoutine = null;
            yield break;
        }
        
        if (destinationRoom == null || !destinationRoom.cleared)
        {
            changingRoomRoutine = null;
            yield break;
        }

        destinationSpriteRenderer = destinationRoom.minimapObject.GetComponentInChildren<SpriteRenderer>();

        destinationSpritePreviousColor = destinationSpriteRenderer.color;
        destinationSpriteRenderer.color = Color.green;
        yield return new WaitForSeconds(0.1f);
        destinationSpriteRenderer.color = destinationSpritePreviousColor;

        destinationSpriteRenderer = null;

        distanceValues = new Dictionary<Room, int> { { destinationRoom, 0 } };
        AssignDistanceValuesForAdjacentRooms(destinationRoom, 1);

        List<Vector3> path = GetPath(currentRoom);
        path.Add(playerRepresantation.position);
        path.Reverse();

        LineRenderer line = gameObject.AddComponent<LineRenderer>();
        line.enabled = showQuickTravelPath;
        line.sortingLayerName = "Minimap";
        line.sortingOrder = 1;
        line.material = quickTravelPathMaterial;
        line.startColor = quickTravelPathColor;
        line.endColor = quickTravelPathColor;
        line.startWidth = quickTravelPathWidth;
        line.endWidth = quickTravelPathWidth;
        line.positionCount = path.Count;
        line.SetPositions(path.ToArray());

        yield return FollowPath(path, line);

        Destroy(line);

        player.transform.position = playerRepresantation.transform.position * new Vector2(roomSizeInTiles.x, roomSizeInTiles.y);
        
        changingRoomRoutine = null;
        SwitchMode();
    }

    private void AssignDistanceValuesForAdjacentRooms(Room room, int distance)
    {
        List<Room> roomsToCheck = new List<Room>();

        foreach (Room adjacentRoom in room.connectedRooms)
        {
            if (!adjacentRoom.cleared)
                continue;

            if (!distanceValues.ContainsKey(adjacentRoom))
            {
                distanceValues.Add(adjacentRoom, distance);
                roomsToCheck.Add(adjacentRoom);
            }
            else if (distanceValues[adjacentRoom] > distance)
            {
                distanceValues[adjacentRoom] = distance;
                roomsToCheck.Add(adjacentRoom);
            }
        }

        foreach (Room roomToCheck in roomsToCheck)
        {
            AssignDistanceValuesForAdjacentRooms(roomToCheck, distance + 1);
        }
    }

    private List<Vector3> GetPath(Room room)
    {
        if (distanceValues[room] == 0)
            return new List<Vector3>();

        foreach (Room adjacentRoom in room.connectedRooms)
        {
            if (!distanceValues.ContainsKey(adjacentRoom))
                continue;

            if (distanceValues[adjacentRoom] < distanceValues[room])
            {
                List<Vector3> path = GetPath(adjacentRoom);

                if (path != null)
                {
                    Vector3 corridorEntranceOffset = Vector3.zero;
                    Vector3 corridorExitOffset = Vector3.zero;

                    if (room.data.isBossRoom)
                    {
                        Dictionary<Vector2Int, Vector2Int> connections = room.GetComponent<BossRoom>().GetBossRoomData().GetDoorConnections();

                        foreach (Vector2Int connection in connections.Keys)
                        {
                            Vector2Int adjacentRoomPosition = room.data.location + connection;
                            if (GameController.Instance.level[adjacentRoomPosition.x, adjacentRoomPosition.y] == adjacentRoom)
                            {
                                corridorEntranceOffset = (Vector2)connection - (Vector2)connections[connection] * 0.6f;
                                corridorExitOffset = (Vector2)connection - (Vector2)connections[connection] * 0.4f;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (Vector2Int connection in room.data.GetDoorConnections())
                        {
                            Vector2Int adjacentRoomPosition = room.data.location + connection;
                            if (GameController.Instance.level[adjacentRoomPosition.x, adjacentRoomPosition.y] == adjacentRoom)
                            {
                                Vector2 direction = RoomData.GetDoorNormal(connection, room.data.roomType);
                                corridorEntranceOffset = (Vector2)connection - direction * 0.6f;
                                corridorExitOffset = (Vector2)connection - direction * 0.4f;
                                break;
                            }
                        }
                    }

                    Vector3 corridorEntrance = room.minimapObject.transform.position + corridorEntranceOffset;
                    Vector3 corridorExit = room.minimapObject.transform.position + corridorExitOffset;

                    path.Add(corridorExit);
                    path.Add(corridorEntrance);

                    return path;
                }
            }
        }

        return null;
    }

    private IEnumerator FollowPath(List<Vector3> path, LineRenderer line)
    {
        if (path.Count == 1)
            yield break;

        float duration = (path[1] - playerRepresantation.position).magnitude / quickTravelSpeed;
        float timer = duration;
        Vector3 startPosition = playerRepresantation.position;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            playerRepresantation.position = Vector3.Lerp(path[1], startPosition, timer / duration);
            path[0] = playerRepresantation.position;
            line.SetPosition(0, path[0]);
            yield return new WaitForEndOfFrame();
        }

        path.RemoveAt(1);
        line.positionCount = path.Count;
        line.SetPositions(path.ToArray());
        yield return FollowPath(path, line);
    }
}