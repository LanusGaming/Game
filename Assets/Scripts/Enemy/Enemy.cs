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
    public Attack onDeath;
    public float contactDamageCooldown;

    [HideInInspector]
    public bool canMove;

    private Player player;
    private Rigidbody2D rb;

    private bool inContact;
    private bool contactDamageOnCooldown;

    private void Start()
    {
        health = maxHealth;
        player = Player.instance;
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(Attack());
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inContact = true;

            if (!contactDamageOnCooldown)
                StartCoroutine(ContactDamageRoutine());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inContact = false;
        }
    }

    private IEnumerator ContactDamageRoutine()
    {
        contactDamageOnCooldown = true;

        while (inContact)
        {
            player.TakeDamage(damage);
            yield return new WaitForSeconds(contactDamageCooldown);
        }

        contactDamageOnCooldown = false;
    }

    private void Move()
    {
        if (canMove)
            behaviour.Move(this, player);

        if (rb.velocity.x > 0)
            spriteRenderer.flipX = false;
        
        if (rb.velocity.x < 0)
            spriteRenderer.flipX = true;
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            Attack attack = null;
            yield return new WaitUntil(() => (attack = ChooseAttack(GetAvailableAttacks())) is not null);

            StartCoroutine(attack.Invoke(this, player));

            yield return new WaitWhile(() => attack.executing);
            yield return new WaitForSeconds(attack.recovoryTime);
        }
    }

    private Attack[] GetAvailableAttacks()
    {
        List<Attack> available = new List<Attack>();

        foreach (var attack in attacks)
        {
            if (attack.Ready(this, player))
                available.Add(attack);
        }

        return available.ToArray();
    }

    private Attack ChooseAttack(Attack[] attacks)
    {
        float maxProbability = 0f;

        foreach (var attack in attacks)
            maxProbability += attack.probability;

        double value = GameController.combatRandomizer.NextDouble() * maxProbability;

        foreach (var attack in attacks)
        {
            if (value < attack.probability)
                return attack;

            value -= attack.probability;
        }

        return null;
    }

    protected override IEnumerator Die()
    {
        if (onDeath is not null)
        {
            StartCoroutine(onDeath.Invoke(this, player));
            yield return new WaitWhile(() => onDeath.executing);
        }

        Destroy(gameObject);
    }
}
