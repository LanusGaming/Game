using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    public Trigger interactTrigger;
    public GameObject interactDialog;
    public GameObject transitionObject;

    public static int buildIndex;
    
    void Start()
    {
        buildIndex = SceneManager.GetActiveScene().buildIndex;

        if (debugSeed != 0)
            Configuration.game.seed = debugSeed;
        else if (Configuration.game.seed == 0)
            Configuration.game.seed = (int)(DateTime.Now.Ticks % int.MaxValue);

        startGameTrigger.triggeredCallback = StartGame;

        player.active = false;

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.doneCallback = (transition) => {
            player.active = true;
            Destroy(transition.gameObject);
        };

        GameObject dialog = null;

        interactTrigger.interactionEnteredCallback = (trigger) => dialog = UIController.instance.DisplayDialog(interactDialog, player.transform.position);
        interactTrigger.interactionActiveCallback = (trigger) =>
        {
            dialog.transform.localPosition = Camera.main.WorldToScreenPoint(player.transform.position) - new Vector3(Screen.width / 2f, Screen.height / 2f);
        };
        interactTrigger.interactionExitedCallback = (trigger) =>
        {
            if (dialog)
                Destroy(dialog);
        };

        interactTrigger.triggeredCallback = (trigger) =>
        {

            Debug.Log("Interacted!");
        };
    }

    public void StartGame(Trigger trigger)
    {
        player.active = false;

        GameController.generationRandomizer = new System.Random(Configuration.game.seed);
        Debug.Log($"Seed: {Configuration.game.seed}");
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
