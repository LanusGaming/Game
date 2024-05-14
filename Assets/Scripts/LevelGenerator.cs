using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int roomTileSize = new Vector2Int(20, 20);

    public int minimumRooms = 5;
    public int maximumRooms = 15;
    public int minimumDistanceFromBossRoom = 3;

    public float type1Probability = 0.25f;
    public float type2HProbability = 0.25f;
    public float type2VProbability = 0.25f;
    public float type4Probability = 0.25f;

    public float normalRoomProbability = 1f;

    public Transform roomsParent;
    public GameObject[] bossRoomObjects;
    public GameObject[] roomObjects;

    private Room[,] level;
    private Vector2Int levelSize;
    private int roomCount;

    private Dictionary<Room, int> roomToObjectMap;

    private Queue<Room> generationQueue;

    private Room[] spawnRooms;
    private Room[] type1Rooms;
    private Room[] type2HRooms;
    private Room[] type2VRooms;
    private Room[] type4Rooms;

    private int openDoorCount;

    public int Generate(int seed)
    {
        ResetGeneration();

        UnityEngine.Random.InitState(seed);

        roomToObjectMap = new Dictionary<Room, int>();
        Room[] rooms = new Room[roomObjects.Length];

        for (int i = 0; i < roomObjects.Length; i++)
        {
            rooms[i] = roomObjects[i].GetComponent<Room>();
            roomToObjectMap.Add(rooms[i], i);
        }

        CategorizeRooms(rooms);

        levelSize = new Vector2Int((maximumRooms + 1) * 4, (maximumRooms + 1) * 4);
        level = new Room[levelSize.x, levelSize.y];
        generationQueue = new Queue<Room>();
        roomsParent.position = Vector3.zero;

        BossRoom bossRoom = AddBossRoom();

        // Generate rooms around other rooms
        while (generationQueue.Count > 0)
        {
            Room currentRoom = generationQueue.Dequeue();
            if (!GenerateAdjacentRooms(currentRoom))
            {
                Debug.Log($"Restarting generation with seed {seed + 1}");
                return Generate(seed + 1);
            }
        }

        if (!AddSpawnRoom(bossRoom))
        {
            Debug.Log($"Could not generate starting room for seed {seed}");
            Debug.Log($"Restarting generation with seed {seed + 1}");
            return Generate(seed + 1);
        }

        Debug.Log($"Generated a level with {roomCount} rooms! (Seed: {seed})");

        return seed;
    }

    public void ResetGeneration()
    {
        List<GameObject> generatedRooms = new List<GameObject>();

        for (int i = 0; i < roomsParent.childCount; i++)
        {
            generatedRooms.Add(roomsParent.GetChild(i).gameObject);
        }

        foreach (GameObject roomObject in generatedRooms)
        {
            Destroy(roomObject);
        }
    }

    private void CategorizeRooms(Room[] rooms)
    {
        List<Room> spawnRooms = new List<Room>();
        List<Room> type1Rooms = new List<Room>();
        List<Room> type2HRooms = new List<Room>();
        List<Room> type2VRooms = new List<Room>();
        List<Room> type4Rooms = new List<Room>();

        foreach (Room room in rooms)
        {
            switch (room.roomType)
            {
                case RoomType.Type1:
                    if (room.isSpawnRoom)
                        spawnRooms.Add(room);
                    else
                        type1Rooms.Add(room);
                    break;

                case RoomType.Type2Horizontal:
                    type2HRooms.Add(room);
                    break;

                case RoomType.Type2Vertical:
                    type2VRooms.Add(room);
                    break;

                case RoomType.Type4:
                    type4Rooms.Add(room);
                    break;
            }
        }

        this.spawnRooms = spawnRooms.ToArray();
        this.type1Rooms = type1Rooms.ToArray();
        this.type2HRooms = type2HRooms.ToArray();
        this.type2VRooms = type2VRooms.ToArray();
        this.type4Rooms = type4Rooms.ToArray();
    }

    private BossRoom AddBossRoom()
    {
        BossRoom bossRoom = Instantiate(bossRoomObjects[UnityEngine.Random.Range(0, bossRoomObjects.Length)]).GetComponent<BossRoom>();
        bossRoom.transform.SetParent(roomsParent);

        openDoorCount = bossRoom.GetDoorCount() - 1;

        Vector2Int offset = Vector2Int.zero;

        switch (bossRoom.bottomLeft.roomType)
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

        // bottom-left tile
        Room bottomLeftTile = bossRoom.bottomLeft;
        bottomLeftTile.location = levelSize/2;
        AssignRoomToLevel(bottomLeftTile);
        generationQueue.Enqueue(bottomLeftTile);

        // bottom-right tile
        Room bottomRightTile = bossRoom.bottomRight;
        bottomRightTile.location = new Vector2Int(levelSize.x / 2 + offset.x, levelSize.y / 2);
        AssignRoomToLevel(bottomRightTile);
        generationQueue.Enqueue(bottomRightTile);

        // top-left tile
        Room topLeftTile = bossRoom.topLeft;
        topLeftTile.location = new Vector2Int(levelSize.x / 2, levelSize.y / 2 + offset.y);
        AssignRoomToLevel(topLeftTile);
        generationQueue.Enqueue(topLeftTile);

        // top-right tile
        Room topRightTile = bossRoom.topRight;
        topRightTile.location = levelSize / 2 + offset;
        AssignRoomToLevel(topRightTile);
        generationQueue.Enqueue(topRightTile);

        // create exit room
        GameObject exitRoomObject = Instantiate(bossRoom.exitRoomObject);
        Room exitRoom = exitRoomObject.GetComponent<Room>();
        exitRoom.location = levelSize / 2 + bossRoom.exitRoomPosition;
        exitRoom.distanceFromBossRoom = -1;

        exitRoomObject.transform.position = new Vector3(bossRoom.exitRoomPosition.x * roomTileSize.x, bossRoom.exitRoomPosition.y * roomTileSize.y, 0);
        exitRoomObject.transform.SetParent(roomsParent);

        AssignRoomToLevel(exitRoom);

        roomCount = 2;

        return bossRoom;
    }

    private void AssignRoomToLevel(Room room)
    {
        switch (room.roomType)
        {
            case RoomType.Type1:
                level[room.location.x, room.location.y] = room;
                break;

            case RoomType.Type2Horizontal:
                level[room.location.x, room.location.y] = room;
                level[room.location.x + 1, room.location.y] = room;
                break;

            case RoomType.Type2Vertical:
                level[room.location.x, room.location.y] = room;
                level[room.location.x, room.location.y + 1] = room;
                break;

            case RoomType.Type4:
                level[room.location.x, room.location.y] = room;
                level[room.location.x + 1, room.location.y] = room;
                level[room.location.x, room.location.y + 1] = room;
                level[room.location.x + 1, room.location.y + 1] = room;
                break;
        }
    }

    private bool GenerateAdjacentRooms(Room room)
    {
        Dictionary<Vector2Int, Vector2Int> connections = room.GetDoorConnections();

        foreach (Vector2Int connectionDirection in connections.Keys)
        {
            if (level[room.location.x + connectionDirection.x, room.location.y + connectionDirection.y] != null)
                continue;

            bool generatedRoom = false;

            foreach (RoomType roomType in GetRoomTypePriority(room.location + connectionDirection))
            {
                foreach (Vector2Int offset in GetOffsetPriority(roomType, room.location + connectionDirection))
                {
                    Dictionary<Room, int> fittingRooms = FindFittingRooms(roomType, room.location + connectionDirection + offset);

                    if (fittingRooms.Count == 0)
                        continue;

                    Room fittingRoom = fittingRooms.Keys.ToArray()[UnityEngine.Random.Range(0, fittingRooms.Count)];

                    openDoorCount += fittingRooms[fittingRoom];
                    roomCount++;

                    generationQueue.Enqueue(CreateRoom(fittingRoom, room.location + connectionDirection + offset));

                    generatedRoom = true;
                    break;
                }

                if (generatedRoom)
                    break;
            }

            if (!generatedRoom)
            {
                Debug.Log($"Could not generate room for {room.location + connectionDirection} from {room.location}!");
                return false;
            }
        }

        return true;
    }

    private Room CreateRoom(Room room, Vector2Int location)
    {
        GameObject newRoomObject = Instantiate(roomObjects[roomToObjectMap[room]]);
        room = newRoomObject.GetComponent<Room>();
        room.location = location;
        room.distanceFromBossRoom = -1;

        Vector2Int roomPosition = (room.location - levelSize / 2);
        newRoomObject.transform.position = new Vector3(roomPosition.x * roomTileSize.x, roomPosition.y * roomTileSize.y, 0);
        newRoomObject.transform.SetParent(roomsParent);

        AssignRoomToLevel(room);

        return room;
    }

    private RoomType[] GetRoomTypePriority(Vector2Int location)
    {
        Dictionary<RoomType, float> possibleRoomTypes = new Dictionary<RoomType, float>();
        float maxProbability = 0;

        if (GetAvailableSpace(RoomType.Type1, location).Length > 0)
        {
            possibleRoomTypes.Add(RoomType.Type1, type1Probability);
            maxProbability += type1Probability;
        }


        if (GetAvailableSpace(RoomType.Type2Horizontal, location).Length > 0)
        {
            possibleRoomTypes.Add(RoomType.Type2Horizontal, type2HProbability);
            maxProbability += type2HProbability;
        }


        if (GetAvailableSpace(RoomType.Type2Vertical, location).Length > 0)
        {
            possibleRoomTypes.Add(RoomType.Type2Vertical, type2VProbability);
            maxProbability += type2VProbability;
        }


        if (GetAvailableSpace(RoomType.Type4, location).Length > 0)
        {
            possibleRoomTypes.Add(RoomType.Type4, type4Probability);
            maxProbability += type4Probability;
        }

        RoomType[] roomTypePriority = new RoomType[possibleRoomTypes.Count];

        for (int i = 0; i < roomTypePriority.Length; i++)
        {
            float random = UnityEngine.Random.value * maxProbability;

            foreach (RoomType type in possibleRoomTypes.Keys)
            {
                if (random < possibleRoomTypes[type])
                {
                    roomTypePriority[i] = type;
                    maxProbability -= possibleRoomTypes[type];
                    possibleRoomTypes.Remove(type);
                    break;
                }

                random -= possibleRoomTypes[type];
            }
        }

        return roomTypePriority;
    }

    private Vector2Int[] GetOffsetPriority(RoomType roomType, Vector2Int location)
    {
        List<Vector2Int> availableOffsets = new List<Vector2Int>();
        availableOffsets.AddRange(GetAvailableSpace(roomType, location));

        Vector2Int[] offsetPriority = new Vector2Int[availableOffsets.Count];

        for (int i = 0; i < offsetPriority.Length; i++)
        {
            float random = UnityEngine.Random.value * availableOffsets.Count;

            foreach (Vector2Int offset in availableOffsets)
            {
                if (random < 1f)
                {
                    offsetPriority[i] = offset;
                    availableOffsets.Remove(offset);
                    break;
                }

                random--;
            }
        }

        return offsetPriority;
    }

    private Vector2Int[] GetAvailableSpace(RoomType roomType, Vector2Int location)
    {
        List<Vector2Int> available = new List<Vector2Int>();

        switch (roomType)
        {
            case RoomType.Type1:
                available.Add(Vector2Int.zero);
                break;

            case RoomType.Type2Horizontal:
                if (level[location.x + 1, location.y] is null)
                    available.Add(Vector2Int.zero);
                if (level[location.x - 1, location.y] is null)
                    available.Add(Vector2Int.left);
                break;

            case RoomType.Type2Vertical:
                if (level[location.x, location.y + 1] is null)
                    available.Add(Vector2Int.zero);
                if (level[location.x, location.y - 1] is null)
                    available.Add(Vector2Int.down);
                break;

            case RoomType.Type4:
                if (level[location.x, location.y + 1] is null && level[location.x + 1, location.y] is null && level[location.x + 1, location.y + 1] is null)
                    available.Add(Vector2Int.zero);
                if (level[location.x + 1, location.y] is null && level[location.x, location.y - 1] is null && level[location.x + 1, location.y - 1] is null)
                    available.Add(Vector2Int.down);
                if (level[location.x, location.y - 1] is null && level[location.x - 1, location.y] is null && level[location.x - 1, location.y - 1] is null)
                    available.Add(new Vector2Int(-1, -1));
                if (level[location.x - 1, location.y] is null && level[location.x, location.y + 1] is null && level[location.x - 1, location.y + 1] is null)
                    available.Add(Vector2Int.left);
                break;
        }

        return available.ToArray();
    }

    private Dictionary<Room, int> FindFittingRooms(RoomType roomType, Vector2Int location)
    {
        // TODO add categorization between normal rooms and other types to change probability of them being chosen
        List<Room> roomShapes = new List<Room>();

        switch (roomType)
        {
            case RoomType.Type1:
                roomShapes.AddRange(type1Rooms);
                break;

            case RoomType.Type2Horizontal:
                roomShapes.AddRange(type2HRooms);
                break;

            case RoomType.Type2Vertical:
                roomShapes.AddRange(type2VRooms);
                break;

            case RoomType.Type4:
                roomShapes.AddRange(type4Rooms);
                break;
        }

        Dictionary<Room, int> fittingRooms = new Dictionary<Room, int>();

        foreach (Room currentRoom in roomShapes)
        {
            Dictionary<Vector2Int, Vector2Int> connections = currentRoom.GetDoorConnections();

            bool fits = true;
            int openDoors = currentRoom.GetDoorCount();

            foreach (Vector2Int adjacentRoomOffset in Room.GetSurroundings(roomType))
            {
                Room adjacentRoom = level[location.x + adjacentRoomOffset.x, location.y + adjacentRoomOffset.y];

                List<Vector2Int> connectionsFromCurrentRoom = new List<Vector2Int>();
                List<Vector2Int> connectionsFromAdjacentRoom = new List<Vector2Int>();

                if (adjacentRoom is null)
                    continue;

                foreach (Vector2Int adjacentRoomConnectionDirection in adjacentRoom.GetDoorConnections().Keys)
                {
                    switch (roomType)
                    {
                        case RoomType.Type1:
                            if (adjacentRoom.location + adjacentRoomConnectionDirection == location)
                                connectionsFromAdjacentRoom.Add(adjacentRoom.location + adjacentRoomConnectionDirection);
                            break;

                        case RoomType.Type2Horizontal:
                            if (adjacentRoom.location + adjacentRoomConnectionDirection == location || adjacentRoom.location + adjacentRoomConnectionDirection == location + Vector2Int.right)
                                connectionsFromAdjacentRoom.Add(adjacentRoom.location + adjacentRoomConnectionDirection);
                            break;

                        case RoomType.Type2Vertical:
                            if (adjacentRoom.location + adjacentRoomConnectionDirection == location || adjacentRoom.location + adjacentRoomConnectionDirection == location + Vector2Int.up)
                                connectionsFromAdjacentRoom.Add(adjacentRoom.location + adjacentRoomConnectionDirection);
                            break;

                        case RoomType.Type4:
                            if (adjacentRoom.location + adjacentRoomConnectionDirection == location || adjacentRoom.location + adjacentRoomConnectionDirection == location + Vector2Int.right || adjacentRoom.location + adjacentRoomConnectionDirection == location + Vector2Int.up || adjacentRoom.location + adjacentRoomConnectionDirection == location + new Vector2Int(1, 1))
                                connectionsFromAdjacentRoom.Add(adjacentRoom.location + adjacentRoomConnectionDirection);
                            break;
                    }
                }

                foreach (Vector2Int connectionDirection in connections.Keys)
                {
                    switch (adjacentRoom.roomType)
                    {
                        case RoomType.Type1:
                            if (location + connectionDirection == adjacentRoom.location)
                                connectionsFromCurrentRoom.Add(location + connectionDirection - connections[connectionDirection]);
                            break;

                        case RoomType.Type2Horizontal:
                            if (location + connectionDirection == adjacentRoom.location || location + connectionDirection == adjacentRoom.location + Vector2Int.right)
                                connectionsFromCurrentRoom.Add(location + connectionDirection - connections[connectionDirection]);
                            break;

                        case RoomType.Type2Vertical:
                            if (location + connectionDirection == adjacentRoom.location || location + connectionDirection == adjacentRoom.location + Vector2Int.up)
                                connectionsFromCurrentRoom.Add(location + connectionDirection - connections[connectionDirection]);
                            break;

                        case RoomType.Type4:
                            if (location + connectionDirection == adjacentRoom.location || location + connectionDirection == adjacentRoom.location + Vector2Int.right || location + connectionDirection == adjacentRoom.location + Vector2Int.up || location + connectionDirection == adjacentRoom.location + new Vector2Int(1, 1))
                                connectionsFromCurrentRoom.Add(location + connectionDirection - connections[connectionDirection]);
                            break;
                    }
                }

                if (connectionsFromAdjacentRoom.Count + connectionsFromCurrentRoom.Count == 0)
                    continue;

                if (connectionsFromAdjacentRoom.Count - connectionsFromCurrentRoom.Count != 0)
                {
                    fits = false;
                    break;
                }

                foreach (Vector2Int adjacentConnection in connectionsFromAdjacentRoom)
                {
                    bool contained = false;

                    foreach (Vector2Int currentConnection in connectionsFromCurrentRoom)
                    {
                        if (currentConnection == adjacentConnection)
                        {
                            contained = true;
                            break;
                        }
                    }

                    if (!contained)
                    {
                        fits = false;
                        break;
                    }
                }

                openDoors -= 2;
            }

            if (fits
                && (openDoorCount + openDoors > 0
                    || (roomCount >= minimumRooms && openDoorCount + openDoors == 0))
                && roomCount + openDoorCount + openDoors < maximumRooms)
                fittingRooms.Add(currentRoom, openDoors);
        }

        return fittingRooms;
    }

    private Room[] GetStartingRoomCandidates(BossRoom bossRoom)
    {
        bossRoom.bottomLeft.distanceFromBossRoom = 0;
        bossRoom.bottomRight.distanceFromBossRoom = 0;
        bossRoom.topLeft.distanceFromBossRoom = 0;
        bossRoom.topRight.distanceFromBossRoom = 0;

        AssignDistanceValueForAdjacentRooms(bossRoom.bottomLeft);
        AssignDistanceValueForAdjacentRooms(bossRoom.bottomRight);
        AssignDistanceValueForAdjacentRooms(bossRoom.topLeft);
        AssignDistanceValueForAdjacentRooms(bossRoom.topRight);

        List<Room> candidates = new List<Room>();

        for (int x = 0; x < levelSize.x; x++)
        {
            for (int y = 0; y < levelSize.y; y++)
            {
                if (level[x, y] is null)
                    continue;

                if (level[x, y].distanceFromBossRoom >= minimumDistanceFromBossRoom && !candidates.Contains(level[x, y]))
                    candidates.Add(level[x, y]);
            }
        }

        return candidates.ToArray();
    }

    private void AssignDistanceValueForAdjacentRooms(Room room)
    {
        List<Room> roomsToCheck = new List<Room>();

        foreach (Vector2Int adjecentRoomOffset in room.GetDoorConnections().Keys)
        {
            Room adjacentRoom = level[room.location.x + adjecentRoomOffset.x, room.location.y + adjecentRoomOffset.y];

            if (adjacentRoom.distanceFromBossRoom == -1)
            {
                adjacentRoom.distanceFromBossRoom = room.distanceFromBossRoom + 1;
                roomsToCheck.Add(adjacentRoom);
            }
            else if (adjacentRoom.distanceFromBossRoom > room.distanceFromBossRoom + 1)
            {
                adjacentRoom.distanceFromBossRoom = room.distanceFromBossRoom + 1;
                roomsToCheck.Add(adjacentRoom);
            }
        }

        foreach(Room roomToCheck in roomsToCheck)
        {
            AssignDistanceValueForAdjacentRooms(roomToCheck);
        }
    }

    private bool AddSpawnRoom(BossRoom bossRoom)
    {
        Room[] candidates = GetStartingRoomCandidates(bossRoom);

        foreach (Room room in ShuffleArray(candidates))
        {
            List<Room> fittingRooms = new List<Room>();

            foreach (Room spawnRoom in spawnRooms)
            {
                if (spawnRoom.roomType == room.roomType && spawnRoom.doors == room.doors)
                {
                    fittingRooms.Add(spawnRoom);
                }
            }

            if (fittingRooms.Count == 0)
                continue;

            Room fittingRoom = fittingRooms[UnityEngine.Random.Range(0, fittingRooms.Count)];
            GameObject oldRoom = level[room.location.x, room.location.y].gameObject;

            roomsParent.transform.position = -CreateRoom(fittingRoom, room.location).transform.position;

            Destroy(oldRoom);

            return true;
        }

        return false;
    }
    
    private T[] ShuffleArray<T>(T[] array)
    {
        List<T> result = new List<T>();
        List<T> remaining = new List<T>();
        remaining.AddRange(array);

        while (remaining.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, remaining.Count);
            result.Add(remaining[index]);
            remaining.RemoveAt(index);
        }

        return result.ToArray();
    }
}
