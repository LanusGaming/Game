using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public abstract class Behaviour : MonoBehaviour
{
    public abstract void Move(Enemy enemy, Player player);

    protected void MoveInDirection(Enemy enemy, Vector2 direction)
    {
        enemy.GetComponent<Rigidbody2D>().velocity = direction.normalized * enemy.MoveSpeed;
    }
}
