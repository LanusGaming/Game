using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Player player;

    void Start()
    {
        player = Player.Instance;
    }

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -1);
    }
}
