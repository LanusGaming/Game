using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Attack
{
    public GameObject bulletObject;
    public Material lineMaterial;
    public Color lineColor = Color.red;
    public float minLineThickness = 0.05f;
    public float maxLineThickness = 0.1f;
    public float startupDuration = 2f;
    public float firingDelay = 0.2f;
    public float bulletSpeed = 15f;
    public float spawnDistanceFromEnemy = 0.5f;
    public float maxDistance = 50f;

    public override bool Ready(Enemy enemy, Player player)
    {
        return true;
    }

    protected override IEnumerator Execute(Enemy enemy, Player player)
    {
        enemy.canMove = false;

        LineRenderer line = enemy.gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.material = lineMaterial;
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.startWidth = minLineThickness;

        float timer = 0f;

        while (timer < startupDuration)
        {
            line.SetPositions(new Vector3[] { enemy.transform.position, GetContactPoint(enemy, player) });
            line.startWidth = Mathf.Lerp(minLineThickness, maxLineThickness, timer / startupDuration);

            timer += Time.deltaTime;
            yield return null;
        }

        line.startColor = Color.white;
        line.endColor = Color.white;
        yield return new WaitForSeconds(firingDelay);

        SpawnBullet(enemy, bulletObject, (line.GetPosition(1) - enemy.transform.position).normalized, spawnDistanceFromEnemy, 0f, bulletSpeed, 0f, 0f);
        Destroy(line);

        enemy.canMove = true;
    }

    private Vector2 GetContactPoint(Enemy enemy, Player player)
    {
        Vector3 direction = (player.transform.position - enemy.transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, direction, maxDistance, LayerMask.GetMask("Wall"));

        if (hit.collider != null)
            return hit.point;
        else
            return enemy.transform.position + direction * maxDistance;
    }
}
