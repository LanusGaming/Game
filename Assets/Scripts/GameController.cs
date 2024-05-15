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
    public LevelGenerator levelGenerator;
    public Trigger levelEndTrigger;
    public GameObject transitionObject;

    public static System.Random generationRandomizer;
    public static Queue<int> levelOrder;

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
        
        levelGenerator.Generate();
    }

    public void EndLevel(Trigger trigger)
    {
        player.active = false;

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;
        transition.doneCallback = (transition) => SceneManager.LoadSceneAsync(levelOrder.Dequeue());
        transition.reversed = true;
    }
}
