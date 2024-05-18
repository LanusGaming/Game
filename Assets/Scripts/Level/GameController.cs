using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class GameController : MonoBehaviour
{
    public Player player;
    public Minimap minimap;
    public Trigger levelEndTrigger;
    public GameObject transitionObject;

    [Header("Level Generation")]
    public LevelGenerationProperties properties;
    public Vector2Int roomSizeInTiles;
    public GameObject[] spawnRoomObjects;
    public GameObject[] bossRoomObjects;
    public GameObject[] roomObjects;
    public Transform levelParent;
    public Transform bulletParent;

    [Header("Debug Options")]
    public bool generateLevel = true;
    public bool setRandomizers = false;
    public bool requiresLevelEndTrigger = true;
    public bool playOpeningTransition = true;
    public bool setPlayerStats = false;
    public Stats playerStats;

    public static GameController instance;
    public static System.Random generationRandomizer;
    public static System.Random combatRandomizer;
    public static System.Random generalRandomizer;
    public static Queue<string> levelOrder;

    //private Dictionary<string, GameObject> roomIDToObjectMap;
    private Room[,] level;
    private Vector2Int levelSize;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (setRandomizers)
        {
            generationRandomizer = new System.Random((int)(DateTime.Now.Ticks % int.MaxValue));
            combatRandomizer = new System.Random((int)(DateTime.Now.Ticks % int.MaxValue));
            generalRandomizer = new System.Random((int)(DateTime.Now.Ticks % int.MaxValue));
        }

        if (setPlayerStats)
            PlayerData.stats = playerStats;

        if (requiresLevelEndTrigger)
            levelEndTrigger.triggeredCallback = EndLevel;

        if (playOpeningTransition)
        {
            player.active = false;
            Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
            transition.transform.position = player.transform.position;
            transition.doneCallback = (transition) => {
                player.active = true;
                Destroy(transition.gameObject);
            };
        }

        if (generateLevel)
        {
            level = Generate(properties, levelParent, minimap, generationRandomizer, out levelSize);
        }
    }

    private void Update()
    {
        if (generateLevel)
            minimap.minimapCamera.transform.localPosition = new Vector3(player.transform.position.x / roomSizeInTiles.x, player.transform.position.y / roomSizeInTiles.y, -1f);
    }

    public Room[,] Generate(LevelGenerationProperties properties, Transform levelParent, Minimap minimap, System.Random randomizer, out Vector2Int levelSize)
    {
        Dictionary<string, GameObject> roomIDToObjectMap;
        RoomData[,] levelData = GenerateLevel(properties, randomizer, out levelSize, out roomIDToObjectMap);
        Room[,] level = GenerateLevelObjects(levelData, levelSize, roomIDToObjectMap);
        minimap.GenerateMinimap(level, levelSize);
        ConnectRooms(level, levelSize);

        return level;
    }

    public void EndLevel(Trigger trigger)
    {
        player.active = false;

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.reversed = true;

        transition.doneCallback = (transition) => SceneManager.LoadSceneAsync(levelOrder.Dequeue());
    }

    private RoomData[,] GenerateLevel(LevelGenerationProperties properties, System.Random randomizer, out Vector2Int levelSize, out Dictionary<string, GameObject> roomIDToObjectMap)
    {
        roomIDToObjectMap = new Dictionary<string, GameObject>();

        List<RoomData> spawnRoomsData = new List<RoomData>();
        List<BossRoomData> bossRoomsData = new List<BossRoomData>();
        List<RoomData> roomsData = new List<RoomData>();

        foreach (GameObject spawnRoom in spawnRoomObjects)
        {
            Room spawnRoomScript = spawnRoom.GetComponent<Room>();
            RoomData roomData = spawnRoomScript.data;
            spawnRoomsData.Add(roomData);

            roomData.roomID = spawnRoom.name;
            roomIDToObjectMap.Add(roomData.roomID, spawnRoom);
        }

        foreach (GameObject bossRoom in bossRoomObjects)
        {
            BossRoom bossRoomScript = bossRoom.GetComponent<BossRoom>();
            BossRoomData roomData = bossRoomScript.GetBossRoomData();
            bossRoomsData.Add(roomData);

            roomData.bottomLeft.roomID = bossRoom.name + "_bl";
            roomData.bottomRight.roomID = bossRoom.name + "_br";
            roomData.topLeft.roomID = bossRoom.name + "_tl";
            roomData.topRight.roomID = bossRoom.name + "_tr";
            roomData.exitRoom.roomID = bossRoomScript.exitRoomObject.name;

            roomIDToObjectMap.Add(roomData.bottomLeft.roomID, bossRoom);
            roomIDToObjectMap.Add(roomData.bottomRight.roomID, null);
            roomIDToObjectMap.Add(roomData.topLeft.roomID, null);
            roomIDToObjectMap.Add(roomData.topRight.roomID, null);
            roomIDToObjectMap.Add(roomData.exitRoom.roomID, bossRoomScript.exitRoomObject);
        }

        foreach (GameObject room in roomObjects)
        {
            Room roomScript = room.GetComponent<Room>();
            RoomData roomData = roomScript.data;
            roomsData.Add(roomData);

            roomData.roomID = room.name;
            roomIDToObjectMap.Add(roomData.roomID, room);
        }

        LevelGenerator levelGenerator = new LevelGenerator(properties, spawnRoomsData.ToArray(), bossRoomsData.ToArray(), roomsData.ToArray(), randomizer);

        return levelGenerator.Generate(out levelSize);
    }

    private Room[,] GenerateLevelObjects(RoomData[,] levelData, Vector2Int levelSize, Dictionary<string, GameObject> roomIDToObjectMap)
    {
        Room[,] level = new Room[levelSize.x, levelSize.y];

        for (int x = 0; x < levelSize.x; x++)
        {
            for (int y = 0; y < levelSize.y; y++)
            {
                if (levelData[x, y] is null)
                    continue;

                RoomData roomData = levelData[x, y];

                if (roomData.location != new Vector2Int(x, y))
                {
                    if (roomData.isBossRoom)
                        continue;

                    level[x, y] = level[roomData.location.x, roomData.location.y];
                    continue;
                }

                if (roomIDToObjectMap[roomData.roomID] is null)
                    continue;

                GameObject room = Instantiate(roomIDToObjectMap[roomData.roomID], levelParent);
                room.transform.localPosition = new Vector2(x * roomSizeInTiles.x, y * roomSizeInTiles.y);

                if (room.GetComponent<BossRoom>() != null)
                    FillBossRoom(level, room.GetComponent<BossRoom>(), roomData.location);
                else
                    level[x, y] = room.GetComponent<Room>();

                level[x, y].data = roomData;

                if (roomData.isSpawnRoom)
                    levelParent.position = new Vector2(x * roomSizeInTiles.x, y * roomSizeInTiles.y) * -1;
            }
        }

        return level;
    }

    private void FillBossRoom(Room[,] level, BossRoom bossRoom, Vector2Int location)
    {
        Vector2Int size = bossRoom.GetBossRoomData().GetRoomSize();

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                level[location.x + x, location.y + y] = bossRoom.bottomLeft;
    }

    private void ConnectRooms(Room[,] level, Vector2Int levelSize)
    {
        for (int x = 0; x < levelSize.x; x++)
        {
            for (int y = 0; y < levelSize.y; y++)
            {
                if (level[x, y] == null)
                    continue;

                Room room = level[x, y];

                if (room.data.location != new Vector2Int(x, y))
                    continue;

                List<Room> connectedRooms = new List<Room>();

                Vector2Int[] connections;

                if (room.data.isBossRoom)
                    connections = room.GetComponent<BossRoom>().GetBossRoomData().GetDoorConnections().Keys.ToArray();
                else
                    connections = room.data.GetDoorConnections();

                foreach (Vector2Int connectionDirection in connections)
                {
                    connectedRooms.Add(level[x + connectionDirection.x, y + connectionDirection.y]);
                }

                room.connectedRooms = connectedRooms.ToArray();

                if (room.data.isSpawnRoom)
                {
                    room.room.gameObject.SetActive(true);
                    room.OnVisibilityTriggerHit(null);
                }
                else if (room.room)
                    room.room.gameObject.SetActive(false);
            }
        }
    }
}
