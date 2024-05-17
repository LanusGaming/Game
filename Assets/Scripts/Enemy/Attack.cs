using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    public float damage;
    public float minimumDistanceFromPlayer;
    public float maximumDistanceFromPlayer;
    public bool standStill;
    public float recovoryTime;
    public float cooldown;

    [HideInInspector]
    public bool onCooldown;
    [HideInInspector]
    public bool executing;

    public bool Ready(Enemy enemy, Player player)
    {
        if (onCooldown)
            return false;

        float distance = (player.transform.position - enemy.transform.position).magnitude;

        return distance >= minimumDistanceFromPlayer && ((maximumDistanceFromPlayer > 0) ? distance < maximumDistanceFromPlayer : true);
    }

    public IEnumerator Execute(Enemy enemy, Player player)
    {
        yield return _Execute(enemy, player);
        Done(enemy);
    }

    protected abstract IEnumerator _Execute(Enemy enemy, Player player);

    protected void Done(Enemy enemy)
    {
        onCooldown = true;
        executing = false;

        StartCoroutine(Cooldown());
        enemy.OnAttackFinished(recovoryTime);
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }
}
