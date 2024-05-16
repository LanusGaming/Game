using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

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

    public static System.Random generationRandomizer;
    public static Queue<string> levelOrder;

    void Start()
    {
        levelEndTrigger.callback = EndLevel;

        player.active = false;

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.doneCallback = (transition) => {
            player.active = true;
            Destroy(transition.gameObject);
        };

        GenerateLevel();
    }

    private void Update()
    {
        minimap.minimapCamera.transform.localPosition = new Vector3(player.transform.position.x / roomSizeInTiles.x, player.transform.position.y / roomSizeInTiles.y, -1f);
    }

    public void EndLevel(Trigger trigger)
    {
        player.active = false;

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.reversed = true;

        transition.doneCallback = (transition) => SceneManager.LoadSceneAsync(levelOrder.Dequeue());
    }

    private void GenerateLevel()
    {
        Dictionary<string, GameObject> roomMap = new Dictionary<string, GameObject>();

        List<RoomData> spawnRoomsData = new List<RoomData>();
        List<BossRoomData> bossRoomsData = new List<BossRoomData>();
        List<RoomData> roomsData = new List<RoomData>();

        foreach (GameObject spawnRoom in spawnRoomObjects)
        {
            RoomData roomData = spawnRoom.GetComponent<Room>().data;
            spawnRoomsData.Add(roomData);

            roomData.roomID = spawnRoom.name;
            roomMap.Add(roomData.roomID, spawnRoom);
        }

        foreach (GameObject bossRoom in bossRoomObjects)
        {
            BossRoomData roomData = bossRoom.GetComponent<BossRoom>().GetBossRoomData();
            bossRoomsData.Add(roomData);

            roomData.bottomLeft.roomID = bossRoom.name + "_bl";
            roomData.bottomRight.roomID = bossRoom.name + "_br";
            roomData.topLeft.roomID = bossRoom.name + "_tl";
            roomData.topRight.roomID = bossRoom.name + "_tr";
            roomData.exitRoom.roomID = bossRoom.GetComponent<BossRoom>().exitRoomObject.name;

            roomMap.Add(roomData.bottomLeft.roomID, bossRoom);
            roomMap.Add(roomData.bottomRight.roomID, null);
            roomMap.Add(roomData.topLeft.roomID, null);
            roomMap.Add(roomData.topRight.roomID, null);
            roomMap.Add(roomData.exitRoom.roomID, bossRoom.GetComponent<BossRoom>().exitRoomObject);
        }

        foreach (GameObject room in roomObjects)
        {
            RoomData roomData = room.GetComponent<Room>().data;
            roomsData.Add(roomData);

            roomData.roomID = room.name;
            roomMap.Add(roomData.roomID, room);
        }

        LevelGenerator levelGenerator = new LevelGenerator(properties, spawnRoomsData.ToArray(), bossRoomsData.ToArray(), roomsData.ToArray(), generationRandomizer);
        RoomData[] level = levelGenerator.Generate();

        foreach (RoomData roomData in level)
        {
            if (roomData.isSpawnRoom)
                levelParent.position = new Vector2(roomData.location.x * roomSizeInTiles.x, roomData.location.y * roomSizeInTiles.y) * -1;

            if (roomMap[roomData.roomID] is null)
                continue;

            GameObject room = Instantiate(roomMap[roomData.roomID], levelParent);
            room.transform.localPosition = new Vector2(roomData.location.x * roomSizeInTiles.x, roomData.location.y * roomSizeInTiles.y);
        }

        minimap.GenerateMinimap(level);
    }
}
