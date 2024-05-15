using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct Stage
{
    [SerializeField]
    public string[] levels;
}

public class HUB : MonoBehaviour
{
    public int debugSeed = 0;
    public Stage[] levels;

    public Player player;
    public Trigger startGameTrigger;
    public GameObject transitionObject;

    public static int buildIndex;
    
    void Start()
    {
        buildIndex = SceneManager.GetActiveScene().buildIndex;

        if (debugSeed != 0)
            Settings.seed = debugSeed;
        else if (Settings.seed == 0)
            Settings.seed = (int)(DateTime.Now.Ticks % int.MaxValue);

        startGameTrigger.callback = StartGame;

        player.active = false;

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.doneCallback = (transition) => {
            player.active = true;
            Destroy(transition.gameObject);
        };
    }

    public void StartGame(Trigger trigger)
    {
        player.active = false;

        GameController.generationRandomizer = new System.Random(Settings.seed);
        GenerateLevelOrder();

        player.active = false;

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.doneCallback = (transition) => SceneManager.LoadSceneAsync(GameController.levelOrder.Dequeue());
        transition.reversed = true;
    }

    private void GenerateLevelOrder()
    {
        GameController.levelOrder = new Queue<string>();

        foreach (Stage stage in levels)
        {
            foreach (string level in HelperFunctions.ShuffleArray(stage.levels, GameController.generationRandomizer))
            {
                GameController.levelOrder.Enqueue(level);
            }
        }
    }
}
