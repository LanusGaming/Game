using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Player player;
    public GameObject transitionObject;
    public LevelGenerator levelGenerator;
    public int seed = 1;
    public float duration = 5f;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        seed = levelGenerator.Generate(seed) + 1;
    }

    private void Update()
    {
        if (timer < duration)
            timer += Time.deltaTime;
        else
        {
            seed = levelGenerator.Generate(seed) + 1;
            timer = 0f;
        }
    }

    public void ChangeLevel(Vector2Int direction)
    {
        return;
    }
}
