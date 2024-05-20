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
        get { return PlayerData.health; }
        set { PlayerData.health = value; }
    }
    public override int maxHealth
    {
        get { return PlayerData.maxHealth; }
        set { PlayerData.maxHealth = value; }
    }
    public override float damage
    {
        get { return PlayerData.damage; }
        set { PlayerData.damage = value; }
    }
    public override float moveSpeed
    {
        get { return PlayerData.moveSpeed; }
        set { PlayerData.moveSpeed = value; }
    }
    public override float defense
    {
        get { return PlayerData.defense; }
        set { PlayerData.defense = value; }
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

    protected override IEnumerator Die()
    {
        Debug.Log("Player has died!");
        yield return new WaitForSeconds(invinciblityDuration);
        invincible = false;
    }
}
