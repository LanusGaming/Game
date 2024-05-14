using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int roomTileSize = new Vector2Int(20, 20);

    public int minimumRooms = 5;
    public int maximumRoomsBeforeEndingRoom = 10;
    public int maximumRooms = 15;

    public float type1Probability = 0.25f;
    public float type2HProbability = 0.25f;
    public float type2VProbability = 0.25f;
    public float type4Probability = 0.25f;

    public float normalRoomProbability = 1f;

    public Transform roomsParent;
    public GameObject endingRoomObject;
    public GameObject[] roomObjects;

    private Room[,] level;
    private int roomCount;

    private Dictionary<Room, int> roomToObjectMap;

    private Queue<Room> generationQueue;

    private Room[] startingRooms;
    private Room[] type1Rooms;
    private Room[] type2HRooms;
    private Room[] type2VRooms;
    private Room[] type4Rooms;

    private int openDoorCount;
    private bool startingRoomGenerated;

    public void Generate(int seed)
    {
        UnityEngine.Random.InitState(seed);

        roomToObjectMap = new Dictionary<Room, int>();
        Room[] rooms = new Room[roomObjects.Length];

        for (int i = 0; i < roomObjects.Length; i++)
        {
            rooms[i] = roomObjects[i].GetComponent<Room>();
            roomToObjectMap.Add(rooms[i], i);
        }

        CategorizeRooms(rooms);

        // Size hard coded for now
        level = new Room[maximumRooms*2+1, maximumRooms * 2 + 1];
        generationQueue = new Queue<Room>();    

        Room endingRoom = Instantiate(endingRoomObject).GetComponent<Room>();
        endingRoom.transform.SetParent(roomsParent);
        endingRoom.location = new Vector2Int(maximumRooms, maximumRooms);
        level[maximumRooms, maximumRooms] = endingRoom;
        level[maximumRooms+1, maximumRooms] = endingRoom;
        level[maximumRooms, maximumRooms+1] = endingRoom;
        level[maximumRooms+1, maximumRooms+1] = endingRoom;
        openDoorCount = endingRoom.GetDoorCount();
        startingRoomGenerated = false;
        roomCount++;

        generationQueue.Enqueue(endingRoom);

        while (generationQueue.Count > 0)
        {
            Room currentRoom = generationQueue.Dequeue();
            GenerateAdjacentRooms(currentRoom);
        }
    }

    private void CategorizeRooms(Room[] rooms)
    {
        List<Room> startingRooms = new List<Room>();
        List<Room> type1Rooms = new List<Room>();
        List<Room> type2HRooms = new List<Room>();
        List<Room> type2VRooms = new List<Room>();
        List<Room> type4Rooms = new List<Room>();

        foreach (Room room in rooms)
        {
            switch (room.roomType)
            {
                case RoomType.Type1:
                    if (room.isStartingRoom)
                        startingRooms.Add(room);
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

        this.startingRooms = startingRooms.ToArray();
        this.type1Rooms = type1Rooms.ToArray();
        this.type2HRooms = type2HRooms.ToArray();
        this.type2VRooms = type2VRooms.ToArray();
        this.type4Rooms = type4Rooms.ToArray();
    }

    private void GenerateAdjacentRooms(Room room)
    {
        // TODO Change algorithm to walk for a specific amount of rooms, then create the starting room there, and then go through all the queued rooms
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

                    GameObject newRoomObject = Instantiate(roomObjects[roomToObjectMap[fittingRoom]]);
                    fittingRoom = newRoomObject.GetComponent<Room>();
                    fittingRoom.location = room.location + connectionDirection + offset;

                    Vector2Int roomPosition = (fittingRoom.location - new Vector2Int(maximumRooms, maximumRooms));
                    newRoomObject.transform.position = new Vector3(roomPosition.x * roomTileSize.x, roomPosition.y * roomTileSize.y, 0);
                    newRoomObject.transform.SetParent(roomsParent);

                    switch (roomType)
                    {
                        case RoomType.Type1:
                            level[fittingRoom.location.x, fittingRoom.location.y] = fittingRoom;
                            break;

                        case RoomType.Type2Horizontal:
                            level[fittingRoom.location.x, fittingRoom.location.y] = fittingRoom;
                            level[fittingRoom.location.x + 1, fittingRoom.location.y] = fittingRoom;
                            break;

                        case RoomType.Type2Vertical:
                            level[fittingRoom.location.x, fittingRoom.location.y] = fittingRoom;
                            level[fittingRoom.location.x, fittingRoom.location.y + 1] = fittingRoom;
                            break;

                        case RoomType.Type4:
                            level[fittingRoom.location.x, fittingRoom.location.y] = fittingRoom;
                            level[fittingRoom.location.x + 1, fittingRoom.location.y] = fittingRoom;
                            level[fittingRoom.location.x, fittingRoom.location.y + 1] = fittingRoom;
                            level[fittingRoom.location.x + 1, fittingRoom.location.y + 1] = fittingRoom;
                            break;
                    }

                    generationQueue.Enqueue(fittingRoom);
                    roomCount++;

                    if (fittingRoom.isStartingRoom)
                        startingRoomGenerated = true;

                    generatedRoom = true;
                    break;
                }

                if (generatedRoom)
                    break;
            }

            if (!generatedRoom)
                throw new Exception($"Could not generate room for {room.location + connectionDirection} from {room.location}!");
        }
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
                if (roomCount < maximumRoomsBeforeEndingRoom || startingRoomGenerated)
                    roomShapes.AddRange(type1Rooms);
                if (roomCount >= minimumRooms && !startingRoomGenerated)
                    roomShapes.AddRange(startingRooms);
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

                if (adjacentRoom is null)
                    continue;

                List<Vector2Int> connectionsFromAdjacentRoom = new List<Vector2Int>();

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

                List<Vector2Int> connectionsFromCurrentRoom = new List<Vector2Int>();

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

            if (fits && (openDoorCount + openDoors > 0 || startingRoomGenerated) && roomCount + openDoorCount + openDoors <= maximumRooms)
                fittingRooms.Add(currentRoom, openDoors);
        }

        return fittingRooms;
    }
}
