using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MinimapLoadingIcon : MonoBehaviour, ILoadingIcon
{
    public Minimap minimapLoadingIconMinimap;
    public float explorationIntervall = 0.1f;
    public float generationCooldown = 0.3f;

    [Header("Minimap Generation")]
    public LevelGenerationProperties properties;
    public Vector2Int roomSizeInTiles;
    public GameObject[] spawnRoomObjects;
    public GameObject[] bossRoomObjects;
    public GameObject[] roomObjects;
    public Transform levelParent;

    public void Show()
    {
        StartCoroutine(ShowMinimapIcon());
    }

    private IEnumerator ShowMinimapIcon()
    {
        minimapLoadingIconMinimap.gameObject.SetActive(true);

        while (true)
        {
            ClearMinimap();

            Vector2Int levelSize;
            Room[,] level = LevelGenerator.Generate(properties, spawnRoomObjects, bossRoomObjects, roomObjects, roomSizeInTiles, levelParent, minimapLoadingIconMinimap, HelperFunctions.GetNewRandomizer(), out levelSize);

            Room spawnRoom = GetSpawnRoom(level, levelSize);
            spawnRoom.OnVisibilityTriggerHit(null);

            List<Room> exploredRooms = new List<Room>();
            exploredRooms.Add(spawnRoom);

            yield return ExploreRooms(spawnRoom.connectedRooms.ToArray(), exploredRooms);

            yield return new WaitForSeconds(generationCooldown);
        }
    }

    private IEnumerator ExploreRooms(Room[] rooms, List<Room> exploredRooms)
    {
        List<Room> next = new List<Room>();

        foreach (Room room in rooms)
        {
            if (exploredRooms.Contains(room))
                continue;

            minimapLoadingIconMinimap.ExploreRoom(room);
            room.OnVisibilityTriggerHit(null);
            exploredRooms.Add(room);
            next.AddRange(room.connectedRooms);
        }

        yield return new WaitForSeconds(explorationIntervall);

        if (next.Count > 0)
            yield return ExploreRooms(next.ToArray(), exploredRooms);
    }

    private Room GetSpawnRoom(Room[,] level, Vector2Int levelSize)
    {
        for (int x = 0; x < levelSize.x; x++)
        {
            for (int y = 0; y < levelSize.y; y++)
            {
                if (level[x, y] is null)
                    continue;

                if (level[x, y].data.isSpawnRoom)
                    return level[x, y];
            }
        }

        return null;
    }

    private void ClearMinimap()
    {
        minimapLoadingIconMinimap.Clear();

        for (int i = 0; i < levelParent.childCount; i++)
        {
            Destroy(levelParent.GetChild(i).gameObject);
        }
    }
}
