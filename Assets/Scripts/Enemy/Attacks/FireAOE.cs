using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAOE : Attack
{
    public GameObject bulletObject;
    public float bulletSpeed = 10f;
    public float bulletAcceleration = 0f;
    public float spawnDistanceFromEnemy = 1f;
    public int bulletsPerBurst = 8;
    public int burstAmount = 5;
    public float burstIntervall = 0.1f;
    public float rotationDegreesPerBurst = 5f;
    public float bulletSpreadAngle = 0f;
    public float bulletLifetime = 0f;

    protected override IEnumerator _Execute(Enemy enemy, Player player)
    {
        Vector2 startDirection = (player.transform.position - enemy.transform.position).normalized;

        for (int i = 0; i < burstAmount; i++)
        {
            Quaternion burstOffset = Quaternion.AngleAxis(rotationDegreesPerBurst * i, Vector3.forward);

            for (int j = 0; j < bulletsPerBurst; j++)
            {
                Quaternion bulletOffset = Quaternion.AngleAxis(360f / bulletsPerBurst * j, Vector3.forward);

                Transform bullet = Instantiate(bulletObject, GameController.instance.bulletParent).transform;
                bullet.localPosition = enemy.transform.position + burstOffset * bulletOffset * startDirection * spawnDistanceFromEnemy;
                bullet.rotation = HelperFunctions.LookTowards(Vector3.zero, burstOffset * bulletOffset * startDirection);
                bullet.rotation *= Quaternion.AngleAxis((float)(GameController.combatRandomizer.NextDouble() * bulletSpreadAngle * 2 - bulletSpreadAngle), Vector3.forward);

                Bullet script = bullet.GetComponent<Bullet>();
                script.damage = damage;
                script.speed = bulletSpeed;
                script.acceleration = bulletAcceleration;
                if (bulletLifetime > 0f) script.lifetime = bulletLifetime;
            }

            yield return new WaitForSeconds(burstIntervall);
        }
    }
}
