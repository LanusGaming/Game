using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Attack
{
    public GameObject bulletObject;
    public float bulletSpeed = 10f;
    public float bulletAcceleration = 0f;
    public float spawnDistanceFromEnemy = 1f;
    public int bulletsPerBurst = 5;
    public int burstAmount = 3;
    public float burstIntervall = 0.5f;
    public float rotationDegreesPerBurst = 0f;
    public float bulletSpreadAngle = 60f;
    public float inaccuracyAngle = 0f;
    public float bulletLifetime = 0f;
    public bool followPlayer = true;

    public override bool Ready(Enemy enemy, Player player)
    {
        return true;
    }

    protected override IEnumerator Execute(Enemy enemy, Player player)
    {
        for (int i = 0; i < burstAmount; i++)
        {
            Vector2 startDirection;

            if (followPlayer)
                startDirection = (player.transform.position - enemy.transform.position).normalized;
            else
                startDirection = Vector2.up;

            Quaternion burstOffset = Quaternion.AngleAxis(rotationDegreesPerBurst * i, Vector3.forward);

            for (int j = 0; j < bulletsPerBurst; j++)
            {
                Quaternion bulletOffset = Quaternion.AngleAxis(-bulletSpreadAngle / 2f + bulletSpreadAngle / bulletsPerBurst * j, Vector3.forward);

                SpawnBullet(enemy, bulletObject, burstOffset * bulletOffset * startDirection, spawnDistanceFromEnemy, inaccuracyAngle, bulletSpeed, bulletAcceleration, bulletLifetime);
            }

            yield return new WaitForSeconds(burstIntervall);
        }
    }
}
