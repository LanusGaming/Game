using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Entity
{
    public static Player instance;

    public override int health
    {
        get { return PlayerData.stats.health; }
        set { PlayerData.stats.health = value; }
    }
    public override int maxHealth
    {
        get { return PlayerData.stats.maxHealth; }
        set { PlayerData.stats.maxHealth = value; }
    }
    public override float damage
    {
        get { return PlayerData.stats.damage; }
        set { PlayerData.stats.damage = value; }
    }
    public override float moveSpeed
    {
        get { return PlayerData.stats.moveSpeed; }
        set { PlayerData.stats.moveSpeed = value; }
    }
    public override float defense
    {
        get { return PlayerData.stats.defense; }
        set { PlayerData.stats.defense = value; }
    }

    [HideInInspector]
    public bool active = false;

    private Rigidbody2D rb;

    void Awake() { instance = this; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 velocity = Vector2.zero;

        if (active)
        {
            if (Input.GetKey(Configuration.Controls.moveUp))
                velocity.y += 1;
            if (Input.GetKey(Configuration.Controls.moveDown))
                velocity.y += -1;
            if (Input.GetKey(Configuration.Controls.moveRight))
            {
                velocity.x += 1;
                spriteRenderer.flipX = false;
            }
            if (Input.GetKey(Configuration.Controls.moveLeft))
            {
                velocity.x += -1;
                spriteRenderer.flipX = true;
            }
        }

        rb.velocity = velocity.normalized * moveSpeed;
    }

    protected override void Die()
    {
        Debug.Log("Player has died!");
    }
}
