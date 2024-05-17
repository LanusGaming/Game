using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Entity
{
    public override int health
    {
        get { return stats.health; }
        set { stats.health = value; }
    }
    public override int maxHealth
    {
        get { return stats.maxHealth; }
        set { stats.maxHealth = value; }
    }
    public override float damage
    {
        get { return stats.damage; }
        set { stats.damage = value; }
    }
    public override float moveSpeed
    {
        get { return stats.moveSpeed; }
        set { stats.moveSpeed = value; }
    }
    public override float defense
    {
        get { return stats.defense; }
        set { stats.defense = value; }
    }

    public Behaviour behaviour;
    public Attack[] attacks;
    public float contactDamage;
    public float contactDamageCooldown;

    private Player player;
    private Rigidbody2D rb;

    private Attack activeAttack;
    private float recoveryTimer = 0f;
    private bool inContact;
    private bool contactDamageOnCooldown;

    private void Start()
    {
        health = maxHealth;
        player = Player.instance;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (activeAttack is not null)
        {
            if (!activeAttack.standStill)
                Move();
            return;
        }

        if (recoveryTimer > 0)
        {
            recoveryTimer -= Time.deltaTime;
            Move();
        }
        else
            Attack();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inContact = true;

            if (!contactDamageOnCooldown)
                StartCoroutine(DoContactDamage());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inContact = false;
        }
    }

    private IEnumerator DoContactDamage()
    {
        contactDamageOnCooldown = true;

        while (inContact)
        {
            player.TakeDamage(contactDamage);
            yield return new WaitForSeconds(contactDamageCooldown);
        }

        contactDamageOnCooldown = false;
    }

    private void Move()
    {
        Vector2 velocity = behaviour.Move(this, player) * moveSpeed;

        if (velocity.x > 0)
            spriteRenderer.flipX = false;
        
        if (velocity.x < 0)
            spriteRenderer.flipX = true;

        rb.velocity = velocity;
    }

    private void Attack()
    {
        List<Attack> available = new List<Attack>();

        foreach (var attack in attacks)
        {
            if (attack.Ready(this, player))
                available.Add(attack);
        }

        if (available.Count == 0)
        {
            Move();
            return;
        }

        activeAttack = available[GameController.combatRandomizer.Next(available.Count)];
        StartCoroutine(activeAttack.Execute(this, player));
    }

    public void OnAttackFinished(float recovoryTime)
    {
        recoveryTimer = recovoryTime;
        activeAttack = null;
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}
