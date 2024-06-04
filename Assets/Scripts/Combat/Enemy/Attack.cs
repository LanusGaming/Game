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
    public float probability = 1f;

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

    protected void SpawnBullet(Enemy enemy, GameObject bulletObject, Vector3 direction, float spawnDistanceFromEnemy, float inaccuracyAngle, float speed, float acceleration, float lifetime)
    {
        Transform bullet = Instantiate(bulletObject, GameController.Instance.bulletParent).transform;
        bullet.localPosition = enemy.transform.position + direction * spawnDistanceFromEnemy;
        bullet.rotation = HelperFunctions.LookTowards(Vector2.zero, direction);
        bullet.rotation *= Quaternion.AngleAxis((float)(GameController.combatRandomizer.NextDouble() * inaccuracyAngle - inaccuracyAngle / 2f), Vector3.forward);

        Bullet script = bullet.GetComponent<Bullet>();
        script.damage = enemy.Damage + damage;
        script.speed = speed;
        script.acceleration = acceleration;
        if (lifetime > 0f) script.lifetime = lifetime;
    }

    protected abstract IEnumerator Execute(Enemy enemy, Player player);
}
