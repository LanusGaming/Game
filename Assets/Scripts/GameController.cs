using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Player player;
    public GameObject transitionObject;
    public LevelGenerator levelGenerator;
    public int seed;

    // Start is called before the first frame update
    void Start()
    {
        levelGenerator.Generate(seed);
        //Setup();
    }

    public void ChangeLevel(Vector2Int direction)
    {
        //StartCoroutine(TransitionLevel(direction));
    }
    /*
    private void Setup()
    {
        layout = new Room[layoutSize.x, layoutSize.y];

        layout[level.x, level.y] = Instantiate(levels[0]).GetComponent<Room>();
        layout[level.x, level.y].level = level;

        player.transform.position = spawnPosition;

        // hard coded for now
        layout[0, 0] = Instantiate(levels[1]).GetComponent<Room>();
        layout[0, 0].gameObject.SetActive(false);
        layout[0, 0].level = new Vector2Int(0, 0);
    }

    private IEnumerator TransitionLevel(Vector2Int direction)
    {
        player.active = false;

        Transition transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.reversed = true;
        transition.transform.position = player.transform.position;

        yield return new WaitUntil(() => transition.timer > transition.duration);

        Vector2Int nextLevel = level + direction;
        Room nextLevelScript = layout[nextLevel.x, nextLevel.y];
        nextLevelScript.gameObject.SetActive(true);
        layout[level.x, level.y].gameObject.SetActive(false);

        LevelTrigger entrance = null;

        foreach (LevelTrigger trigger in nextLevelScript.triggers)
        {
            Debug.Log(nextLevelScript.level + trigger.transitionDirection);

            if (nextLevelScript.level + trigger.transitionDirection == level)
                entrance = trigger;
        }

        player.transform.position = entrance.spawnpoint.position;

        level = nextLevel;

        Destroy(transition.gameObject);

        transition = Instantiate(transitionObject).GetComponent<Transition>();
        transition.transform.position = player.transform.position;

        yield return new WaitUntil(() => transition.timer > transition.duration);

        Destroy(transition.gameObject);

        player.active = true;
    }
    */
}
