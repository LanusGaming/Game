using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public Transform spriteMask;
    public float duration = 1;
    public bool reversed = false;

    public LevelGenerationProperties properties;
    public Minimap minimapLoadingIconMinimap;
    public Transform levelParent;
    public float explorationIntervall = 0.1f;

    public Action<Transition> doneCallback;

    private float timer;
    private float screenRadius;
    private bool done;
    private bool explored = false;

    void Start()
    {
        timer = 0;
        screenRadius = (new Vector2(Screen.width, Screen.height)).magnitude * Camera.main.orthographicSize/Screen.height * 2f + 1;

        if (GameController.instance is not null)
            GameController.instance.minimap.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (reversed)
            spriteMask.localScale = Vector3.one * Mathf.Lerp(screenRadius, 0, timer / duration);
        else
            spriteMask.localScale = Vector3.one * Mathf.Lerp(0, screenRadius, timer / duration);

        if (timer < duration)
            timer += Time.deltaTime;
        else if (!done)
        {
            done = true;
            if (!reversed || GameController.instance is null)
            {
                if (GameController.instance is not null)
                    GameController.instance.minimap.gameObject.SetActive(true);
                doneCallback.Invoke(this);
            }
            else
                StartCoroutine(ShowMinimapIcon());
        }
    }
    
    private IEnumerator ShowMinimapIcon()
    {
        minimapLoadingIconMinimap.gameObject.SetActive(true);

        while (true)
        {
            ClearMinimap();

            Vector2Int levelSize;
            Room[,] level = GameController.instance.Generate(properties, levelParent, minimapLoadingIconMinimap, GameController.generalRandomizer, out levelSize);

            Room spawnRoom = GetSpawnRoom(level, levelSize);
            spawnRoom.OnVisibilityTriggerHit(null);

            explored = false;
            List<Room> exploredRooms = new List<Room>();
            exploredRooms.Add(spawnRoom);

            yield return ExploreRooms(spawnRoom.connectedRooms.ToArray(), exploredRooms);

            yield return new WaitUntil(() => explored);
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
        else
            explored = true;
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
