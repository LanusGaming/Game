using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    public Vector2Int transitionDirection = Vector2Int.zero;
    public Transform spawnpoint;

    private GameController controller;

    void Start()
    {
        controller = FindObjectOfType<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject.tag);

        if (collider.gameObject.tag == "Player")
        {
            controller.ChangeLevel(transitionDirection);
        }
    }
}
