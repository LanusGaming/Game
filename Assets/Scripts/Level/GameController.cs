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
    public Transform bulletParent;
    public float loadNextLevelDelay = 2f;

    [Header("Level Generation")]
    public LevelGenerationProperties properties;
    public Vector2Int roomSizeInTiles;
    public GameObject[] spawnRoomObjects;
    public GameObject[] bossRoomObjects;
    public GameObject[] roomObjects;
    public Transform levelParent;

    [Header("Debug Options")]
    public bool generateLevel = true;
    public bool showAllRooms = false;
    public bool exploreAllRooms = false;
    public bool setRandomizers = false;
    public int seed = 0;
    public bool requiresLevelEndTrigger = true;
    public bool playOpeningTransition = true;
    public bool setPlayerStats = false;
    public Stats playerStats;

    [HideInInspector]
    public Room[,] level;
    [HideInInspector]
    public Vector2Int levelSize;

    public static GameController instance;
    public static System.Random generationRandomizer;
    public static System.Random combatRandomizer;
    public static Queue<string> levelOrder;

    private void Awake()
    {
        instance = this;

        if (setRandomizers)
        {
            generationRandomizer = HelperFunctions.GetNewRandomizer(seed, true);
            combatRandomizer = HelperFunctions.GetNewRandomizer();
        }
    }

    void Start()
    {
        if (setPlayerStats)
        {
            PlayerData.stats = playerStats;
            PlayerData.stats.health = playerStats.maxHealth;
        }

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
            level = LevelGenerator.Generate(properties, spawnRoomObjects, bossRoomObjects, roomObjects, roomSizeInTiles, levelParent, minimap, generationRandomizer, out levelSize, showAllRooms, exploreAllRooms);
        }
    }

    private void Update()
    {

    }

    public void EndLevel(Trigger trigger)
    {
        player.active = false;

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.reversed = true;

        transition.doneCallback = (transition) => StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(loadNextLevelDelay);
        SceneManager.LoadSceneAsync(levelOrder.Dequeue());
    }
}
