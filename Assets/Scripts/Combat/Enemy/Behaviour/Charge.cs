using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : Behaviour
{
    public override void Move(Enemy enemy, Player player)
    {
        MoveInDirection(enemy, player.transform.position - transform.position);
    }
}
