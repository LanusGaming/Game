using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    [Min(0)]
    public float damage;
    [Min(0)]
    public float cooldown;
    [Min(0)]
    public float recovoryTime;
    [Min(0)]
    public float probability;

    [HideInInspector]
    public bool onCooldown;
    [HideInInspector]
    public bool executing;

    public abstract bool Ready(Enemy enemy, Player player);

    public IEnumerator Invoke(Enemy enemy, Player player)
    {
        executing = true;
        yield return Execute(enemy, player);
        executing = false;
        yield return Cooldown();
    }

    private IEnumerator Cooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }

    protected abstract IEnumerator Execute(Enemy enemy, Player player);
}
