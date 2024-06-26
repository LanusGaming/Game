using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine.Audio;
using Game;

public class GameController : MonoBehaviour
{
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
    public bool loadSettings = false;
    public bool setSettings = false;
    public AudioMixer mixer;
    public Settings settings;

    [HideInInspector]
    public Room[,] level;
    [HideInInspector]
    public Vector2Int levelSize;

    public static GameController Instance { get; private set; }
    public static System.Random generationRandomizer;
    public static System.Random combatRandomizer;
    public static Queue<string> levelOrder;

    private Player player;

    private void Awake()
    {
        Instance = this;

        if (loadSettings || setSettings)
        {
            Settings.Mixer = mixer;

            if (loadSettings)
            {
                Settings.SettingsManager.LoadSettings();
                Settings.Apply();
            }
            else if (setSettings)
            {
                Settings.Mixer = mixer;
                Settings.Apply(settings);
            }

            SaveData.Set(new SaveData());
            SaveData.StartRun();
        }

        if (setRandomizers)
        {
            generationRandomizer = HelperFunctions.GetNewRandomizer(seed, true);
            combatRandomizer = HelperFunctions.GetNewRandomizer();
        }
    }

    void Start()
    {
        player = Player.Instance;

        if (setPlayerStats)
        {
            RunData.ActiveRun.playerData.stats = playerStats;
            RunData.ActiveRun.playerData.stats.Health = playerStats.MaxHealth;
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

    private void OnApplicationQuit()
    {
        SaveManager.SaveAll();
    }

    public Room GetRoomAt(Vector2 position)
    {
        if (level == null)
            return null;

        Vector2Int coordinates = new Vector2Int(Mathf.RoundToInt(position.x / roomSizeInTiles.x), Mathf.RoundToInt(position.y / roomSizeInTiles.y)) - new Vector2Int((int)(levelParent.transform.position.x / roomSizeInTiles.x), (int)(levelParent.transform.position.y / roomSizeInTiles.y));
        
        if (coordinates.x < 0 || coordinates.y < 0 || coordinates.x >= levelSize.x || coordinates.y >= levelSize.y)
            return null;

        return level[coordinates.x, coordinates.y];
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
