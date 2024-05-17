using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class RoomData_
{
    public string ID;
    public Vector2Int[] roomTiles;
    public RoomData_ adjacentRooms;
}

public class NewLevelGenerator : MonoBehaviour
{
    private RoomData_[,] level;
    
    public RoomData[,] Generate()
    {
        GameObject t = null;
        Vector2 forward = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        float angle = Mathf.Atan(forward.y/ forward.x) + 90;
        Vector2 up = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        t.transform.parent.transform.rotation = Quaternion.LookRotation(forward, up);

        return null;
    }
}