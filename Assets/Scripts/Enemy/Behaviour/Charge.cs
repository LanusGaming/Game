using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : Behaviour
{
    public override Vector2 Move(Enemy enemy, Player player)
    {
        return (player.transform.position - transform.position).normalized;
    }
}
