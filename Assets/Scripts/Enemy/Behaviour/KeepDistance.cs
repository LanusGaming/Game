using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepDistance : Behaviour
{
    public float minimumDistanceFromPlayer = 5f;
    public float maximumDistanceFromPlayer = 7f;

    public override Vector2 Move(Enemy enemy, Player player)
    {
        Vector2 direction = player.transform.position - transform.position;

        if (direction.magnitude < minimumDistanceFromPlayer)
            direction *= -1;
        else if (direction.magnitude < maximumDistanceFromPlayer || maximumDistanceFromPlayer == 0f)
            direction *= 0;

        return direction.normalized;
    }
}
