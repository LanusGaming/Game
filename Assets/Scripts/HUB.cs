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
    public int[] levels;
}

public class HUB : MonoBehaviour
{
    public Trigger startGameTrigger;
    public Player player;
    public GameObject transitionObject;

    public Stage[] levels;
    
    public int debugSeed = 0;
    
    void Start()
    {
        if (debugSeed != 0)
        {
            Settings.seed = debugSeed;
        }

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
        GameController.levelOrder = new Queue<int>();

        foreach (Stage stage in levels)
        {
            foreach (var level in HelperFunctions.ShuffleArray(stage.levels, GameController.generationRandomizer))
            {
                GameController.levelOrder.Enqueue(level);
            }
        }
    }
}
