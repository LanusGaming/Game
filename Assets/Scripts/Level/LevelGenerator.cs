using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LevelGenerationProperties
{
    [Min(3)]
    public int minimumRooms = 10;
    [Min(3)]
    public int maximumRooms = 15;
    [Min(1)]
    public int minimumDistanceFromBossRoom = 3;

    [Min(0)]
    public float type1Probability = 1f;
    [Min(0)]
    public float type2HProbability = 1f;
    [Min(0)]
    public float type2VProbability = 1f;
    [Min(0)]
    public float type4Probability = 1f;
}

public class LevelGenerator
{
    private LevelGenerationProperties properties;
    private RoomData[] spawnRooms;
    private BossRoomData[] bossRooms; 
    private RoomData[] rooms;
    private System.Random randomizer;

    private RoomData[,] level;
    private Vector2Int levelSize;
    private int roomCount;
    private int openDoorCount;

    private Queue<RoomData> generationQueue;

    private RoomData[] type1Rooms;
    private RoomData[] type2HRooms;
    private RoomData[] type2VRooms;
    private RoomData[] type4Rooms;

    public LevelGenerator(LevelGenerationProperties properties, RoomData[] spawnRooms, BossRoomData[] bossRooms, RoomData[] rooms, System.Random randomizer)
    {
        this.properties = properties;
        this.bossRooms = bossRooms;
        this.spawnRooms = spawnRooms;
        this.rooms = rooms;
        this.randomizer = randomizer;
    }

    public RoomData[,] Generate(out Vector2Int size)
    {
        levelSize = new Vector2Int((properties.maximumRooms + 1) * 4, (properties.maximumRooms + 1) * 4);
        level = new RoomData[levelSize.x, levelSize.y];
        generationQueue = new Queue<RoomData>();
        roomCount = 0;
        openDoorCount = 0;

        RoomData[] bossRoomTiles = AddBossRoom();

        CategorizeRooms(rooms);

        // Generate rooms around other rooms
        while (generationQueue.Count > 0)
        {
            if (!GenerateAdjacentRooms(generationQueue.Dequeue()))
                return Generate(out size);
        }

        if (!AddSpawnRoom(bossRoomTiles))
            return Generate(out size);

        size = levelSize;
        return level;
    }

    private void CategorizeRooms(RoomData[] rooms)
    {
        List<RoomData> type1Rooms = new List<RoomData>();
        List<RoomData> type2HRooms = new List<RoomData>();
        List<RoomData> type2VRooms = new List<RoomData>();
        List<RoomData> type4Rooms = new List<RoomData>();

        foreach (RoomData room in rooms)
        {
            switch (room.roomType)
            {
                case RoomType.Type1:
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

        this.type1Rooms = type1Rooms.ToArray();
        this.type2HRooms = type2HRooms.ToArray();
        this.type2VRooms = type2VRooms.ToArray();
        this.type4Rooms = type4Rooms.ToArray();
    }

    private RoomData[] AddBossRoom()
    {
        BossRoomData bossRoom = bossRooms[randomizer.Next(0, bossRooms.Length)];

        openDoorCount = bossRoom.GetDoorCount();

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

        RoomData bottomLeftTile = CreateRoom(bossRoom.bottomLeft, levelSize / 2);
        generationQueue.Enqueue(bottomLeftTile);

        RoomData bottomRightTile = CreateRoom(bossRoom.bottomRight, new Vector2Int(levelSize.x / 2 + offset.x, levelSize.y / 2));
        generationQueue.Enqueue(bottomRightTile);

        RoomData topLeftTile = CreateRoom(bossRoom.topLeft, new Vector2Int(levelSize.x / 2, levelSize.y / 2 + offset.y));
        generationQueue.Enqueue(topLeftTile);

        RoomData topRightTile = CreateRoom(bossRoom.topRight, levelSize / 2 + offset);
        generationQueue.Enqueue(topRightTile);

        CreateRoom(bossRoom.exitRoom, levelSize / 2 + bossRoom.exitRoomPosition).distanceFromBossRoom = 0;

        roomCount = 1;

        return new RoomData[] { bottomLeftTile, bottomRightTile, topLeftTile, topRightTile };
    }

    private void AssignRoomToLevel(RoomData room)
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

    private RoomData CreateRoom(RoomData room, Vector2Int location)
    {
        RoomData newRoom = new RoomData(room);
        newRoom.location = location;
        AssignRoomToLevel(newRoom);

        return newRoom;
    }

    private bool GenerateAdjacentRooms(RoomData room)
    {
        Vector2Int[] connections = room.GetDoorConnections();

        foreach (Vector2Int connectionDirection in connections)
        {
            if (level[room.location.x + connectionDirection.x, room.location.y + connectionDirection.y] != null)
                continue;

            bool generatedRoom = false;

            foreach (RoomType roomType in GetRoomTypePriority(room.location + connectionDirection))
            {
                foreach (Vector2Int offset in GetOffsetPriority(roomType, room.location + connectionDirection))
                {
                    Dictionary<RoomData, int> fittingRooms = FindFittingRooms(roomType, room.location + connectionDirection + offset);

                    if (fittingRooms.Count == 0)
                        continue;

                    RoomData fittingRoom = fittingRooms.Keys.ToArray()[randomizer.Next(0, fittingRooms.Count)];
                    generationQueue.Enqueue(CreateRoom(fittingRoom, room.location + connectionDirection + offset));

                    openDoorCount += fittingRooms[fittingRoom];
                    roomCount++;

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

    private RoomType[] GetRoomTypePriority(Vector2Int location)
    {
        Dictionary<RoomType, float> possibleRoomTypes = new Dictionary<RoomType, float>();
        float maxProbability = 0;

        if (GetAvailableSpace(RoomType.Type1, location).Length > 0)
        {
            possibleRoomTypes.Add(RoomType.Type1, properties.type1Probability);
            maxProbability += properties.type1Probability;
        }


        if (GetAvailableSpace(RoomType.Type2Horizontal, location).Length > 0)
        {
            possibleRoomTypes.Add(RoomType.Type2Horizontal, properties.type2HProbability);
            maxProbability += properties.type2HProbability;
        }


        if (GetAvailableSpace(RoomType.Type2Vertical, location).Length > 0)
        {
            possibleRoomTypes.Add(RoomType.Type2Vertical, properties.type2VProbability);
            maxProbability += properties.type2VProbability;
        }


        if (GetAvailableSpace(RoomType.Type4, location).Length > 0)
        {
            possibleRoomTypes.Add(RoomType.Type4, properties.type4Probability);
            maxProbability += properties.type4Probability;
        }

        RoomType[] roomTypePriority = new RoomType[possibleRoomTypes.Count];

        for (int i = 0; i < roomTypePriority.Length; i++)
        {
            double random = randomizer.NextDouble() * maxProbability;

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
            double random = randomizer.NextDouble() * availableOffsets.Count;

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

    private Dictionary<RoomData, int> FindFittingRooms(RoomType roomType, Vector2Int location)
    {
        // TODO add categorization between normal rooms and other types to change probability of them being chosen
        List<RoomData> roomShapes = new List<RoomData>();

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

        Dictionary<RoomData, int> fittingRooms = new Dictionary<RoomData, int>();

        foreach (RoomData currentRoom in roomShapes)
        {
            Vector2Int[] connections = currentRoom.GetDoorConnections();

            bool fits = true;
            int openDoors = currentRoom.GetDoorCount();

            foreach (Vector2Int adjacentRoomOffset in RoomData.GetSurroundingTiles(roomType))
            {
                RoomData adjacentRoom = level[location.x + adjacentRoomOffset.x, location.y + adjacentRoomOffset.y];

                if (adjacentRoom is null)
                    continue;

                List<Vector2Int> connectionsFromCurrentRoom = new List<Vector2Int>();
                List<Vector2Int> connectionsFromAdjacentRoom = new List<Vector2Int>();

                foreach (Vector2Int connectionDirection in connections)
                {
                    switch (adjacentRoom.roomType)
                    {
                        case RoomType.Type1:
                            if (location + connectionDirection == adjacentRoom.location)
                                connectionsFromCurrentRoom.Add(location + connectionDirection - RoomData.GetDoorNormal(connectionDirection, roomType));
                            break;

                        case RoomType.Type2Horizontal:
                            if (location + connectionDirection == adjacentRoom.location || location + connectionDirection == adjacentRoom.location + Vector2Int.right)
                                connectionsFromCurrentRoom.Add(location + connectionDirection - RoomData.GetDoorNormal(connectionDirection, roomType));
                            break;

                        case RoomType.Type2Vertical:
                            if (location + connectionDirection == adjacentRoom.location || location + connectionDirection == adjacentRoom.location + Vector2Int.up)
                                connectionsFromCurrentRoom.Add(location + connectionDirection - RoomData.GetDoorNormal(connectionDirection, roomType));
                            break;

                        case RoomType.Type4:
                            if (location + connectionDirection == adjacentRoom.location || location + connectionDirection == adjacentRoom.location + Vector2Int.right || location + connectionDirection == adjacentRoom.location + Vector2Int.up || location + connectionDirection == adjacentRoom.location + new Vector2Int(1, 1))
                                connectionsFromCurrentRoom.Add(location + connectionDirection - RoomData.GetDoorNormal(connectionDirection, roomType));
                            break;
                    }
                }

                foreach (Vector2Int adjacentRoomConnectionDirection in adjacentRoom.GetDoorConnections())
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

                if (connectionsFromAdjacentRoom.Count + connectionsFromCurrentRoom.Count == 0)
                    continue;

                if (connectionsFromAdjacentRoom.Count - connectionsFromCurrentRoom.Count != 0)
                {
                    fits = false;
                    break;
                }

                bool currentConnectionCorrect = false;
                bool adjacentConnectionCorrect = false;

                foreach (Vector2Int currentConnection in connectionsFromCurrentRoom)
                {
                    if (currentConnection == location + adjacentRoomOffset - RoomData.GetDoorNormal(adjacentRoomOffset, roomType))
                    {
                        currentConnectionCorrect = true;
                        break;
                    }
                }

                foreach (Vector2Int adjacentConnection in connectionsFromAdjacentRoom)
                {
                    if (adjacentConnection == location + adjacentRoomOffset - RoomData.GetDoorNormal(adjacentRoomOffset, roomType))
                    {
                        adjacentConnectionCorrect = true;
                        break;
                    }
                }

                if (currentConnectionCorrect != adjacentConnectionCorrect)
                {
                    fits = false;
                    break;
                }

                if (!(currentConnectionCorrect || adjacentConnectionCorrect))
                    continue;

                openDoors -= 2;
            }

            if (fits
                && (openDoorCount + openDoors > 0
                    || (roomCount >= properties.minimumRooms && openDoorCount + openDoors == 0))
                && roomCount + openDoorCount + openDoors < properties.maximumRooms)
                fittingRooms.Add(currentRoom, openDoors);
        }

        return fittingRooms;
    }

    private bool AddSpawnRoom(RoomData[] bossRoomTiles)
    {
        RoomData[] candidates = GetSpawnRoomCandidates(bossRoomTiles);

        foreach (RoomData room in HelperFunctions.ShuffleArray(candidates, randomizer))
        {
            List<RoomData> fittingRooms = new List<RoomData>();

            foreach (RoomData spawnRoom in spawnRooms)
            {
                if (spawnRoom.IsEquivalentTo(room))
                {
                    fittingRooms.Add(spawnRoom);
                }
            }

            if (fittingRooms.Count == 0)
                continue;

            CreateRoom(fittingRooms[randomizer.Next(0, fittingRooms.Count)], room.location);

            return true;
        }

        Debug.Log("Could not generate spawn room!");
        return false;
    }

    private RoomData[] GetSpawnRoomCandidates(RoomData[] bossRoomTiles)
    {
        bossRoomTiles[0].distanceFromBossRoom = 0;
        bossRoomTiles[1].distanceFromBossRoom = 0;
        bossRoomTiles[2].distanceFromBossRoom = 0;
        bossRoomTiles[3].distanceFromBossRoom = 0;

        AssignDistanceValueForAdjacentRooms(bossRoomTiles[0]);
        AssignDistanceValueForAdjacentRooms(bossRoomTiles[1]);
        AssignDistanceValueForAdjacentRooms(bossRoomTiles[2]);
        AssignDistanceValueForAdjacentRooms(bossRoomTiles[3]);

        List<RoomData> candidates = new List<RoomData>();

        for (int x = 0; x < levelSize.x; x++)
        {
            for (int y = 0; y < levelSize.y; y++)
            {
                if (level[x, y] is null)
                    continue;

                if (level[x, y].distanceFromBossRoom >= properties.minimumDistanceFromBossRoom && !candidates.Contains(level[x, y]))
                    candidates.Add(level[x, y]);
            }
        }

        return candidates.ToArray();
    }

    private void AssignDistanceValueForAdjacentRooms(RoomData room)
    {
        List<RoomData> roomsToCheck = new List<RoomData>();

        foreach (Vector2Int adjecentRoomOffset in room.GetDoorConnections())
        {
            RoomData adjacentRoom = level[room.location.x + adjecentRoomOffset.x, room.location.y + adjecentRoomOffset.y];

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

        foreach(RoomData roomToCheck in roomsToCheck)
        {
            AssignDistanceValueForAdjacentRooms(roomToCheck);
        }
    }
}
