using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Attack
{
    public override bool Ready(Enemy enemy, Player player)
    {
        return true;
    }

    protected override IEnumerator Execute(Enemy enemy, Player player)
    {
        throw new System.NotImplementedException();
    }
}
