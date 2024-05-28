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
    public string[] levels;
}

public class HUB : MonoBehaviour
{
    public int debugSeed = 0;
    public float loadNextLevelDelay = 5f;
    public Stage[] levels;

    public Stats playerStats;
    public Player player;
    public Trigger startGameTrigger;
    public Trigger interactTrigger;
    public GameObject interactDialog;
    public GameObject transitionObject;

    public static int buildIndex;
    
    void Start()
    {
        buildIndex = SceneManager.GetActiveScene().buildIndex;

        startGameTrigger.triggeredCallback = StartGame;

        player.active = false;
        PlayerData.stats = playerStats;
        PlayerData.stats.health = PlayerData.stats.maxHealth;

        // show opening transition
        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.doneCallback = (transition) => {
            player.active = true;
            Destroy(transition.gameObject);
        };

        // test for interact dialog
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

    private void OnApplicationQuit()
    {
        SaveManager.SaveAll();
    }

    public void StartGame(Trigger trigger)
    {
        player.active = false;

        if (debugSeed != 0)
            GameData.Config.seed = debugSeed;
        else if (GameData.Config.seed == 0)
            GameData.Config.seed = (int)(DateTime.Now.Ticks % int.MaxValue);

        GameController.generationRandomizer = HelperFunctions.GetNewRandomizer(GameData.Config.seed, true);
        GameController.combatRandomizer = HelperFunctions.GetNewRandomizer();

        GenerateLevelOrder();

        player.active = false;

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.doneCallback = (transition) => StartCoroutine(LoadNextLevel());
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

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(loadNextLevelDelay);
        SceneManager.LoadSceneAsync(GameController.levelOrder.Dequeue());
    }
}
