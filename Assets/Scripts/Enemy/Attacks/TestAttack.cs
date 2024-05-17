using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAttack : Attack
{
    public GameObject bulletObject;
    public float bulletSpeed = 3f;
    public float bulletAcceleration = 0f;
    public float spawnDistanceFromEnemy = 1f;
    public int burstAmount = 3;
    public float burstIntervall = 0.2f;
    public float bulletLifetime = 0f;

    protected override IEnumerator _Execute(Enemy enemy, Player player)
    {
        for (int i = 0; i < burstAmount; i++)
        {
            Transform bullet = Instantiate(bulletObject, GameController.instance.bulletParent).transform;
            bullet.localPosition = enemy.transform.position + (player.transform.position - enemy.transform.position).normalized * spawnDistanceFromEnemy;
            bullet.rotation = HelperFunctions.LookTowards(enemy.transform.position, player.transform.position);

            Bullet script = bullet.GetComponent<Bullet>();
            script.damage = damage;
            script.speed = bulletSpeed;
            script.acceleration = bulletAcceleration;
            if (bulletLifetime > 0f) script.lifetime = bulletLifetime;

            yield return new WaitForSeconds(burstIntervall);
        }
    }
}
