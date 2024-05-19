using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Attack
{
    public GameObject bulletObject;
    public float bulletSpeed = 10f;
    public float bulletAcceleration = 0f;
    public float spawnDistanceFromEnemy = 1f;
    public int burstAmount = 5;
    public float burstIntervall = 0.1f;
    public float inaccuracyAngle = 10f;
    public float bulletLifetime = 0f;

    public override bool Ready(Enemy enemy, Player player)
    {
        return true;
    }

    protected override IEnumerator Execute(Enemy enemy, Player player)
    {
        for (int i = 0; i < burstAmount; i++)
        {
            Transform bullet = Instantiate(bulletObject, GameController.instance.bulletParent).transform;
            bullet.localPosition = enemy.transform.position + (player.transform.position - enemy.transform.position).normalized * spawnDistanceFromEnemy;
            bullet.rotation = HelperFunctions.LookTowards(enemy.transform.position, player.transform.position);
            bullet.rotation *= Quaternion.AngleAxis((float)(GameController.combatRandomizer.NextDouble() * inaccuracyAngle - inaccuracyAngle / 2f), Vector3.forward);

            Bullet script = bullet.GetComponent<Bullet>();
            script.damage = enemy.damage + damage;
            script.speed = bulletSpeed;
            script.acceleration = bulletAcceleration;
            if (bulletLifetime > 0f) script.lifetime = bulletLifetime;

            yield return new WaitForSeconds(burstIntervall);
        }
    }
}
