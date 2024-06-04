using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : Entity
{
    protected override Stats Stats { get => stats; }
    protected override Dictionary<string, StatusEffect> StatusEffects { get => statusEffects; }

    public Stats stats;
    public Behaviour behaviour;
    public Attack[] attacks;
    public Attack onDeath;
    public float contactDamageCooldown;
    public bool canMove = true;

    private Player player;
    private Rigidbody2D rb;
    private Dictionary<string, StatusEffect> statusEffects = new Dictionary<string, StatusEffect>();

    private bool inContact;
    private bool contactDamageOnCooldown;

    private void Start()
    {
        player = Player.Instance;
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(Attack());
    }

    private void Update()
    {
        if (rb.velocity.x > 0.05f)
            spriteRenderer.flipX = false;

        if (rb.velocity.x < 0.05f)
            spriteRenderer.flipX = true;

        if (Math.Abs(rb.velocity.x) < 0.05f)
            spriteRenderer.flipX = (player.transform.position - transform.position).x < 0f;
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
            player.TakeDamage(Damage);
            yield return new WaitForSeconds(contactDamageCooldown);
        }

        contactDamageOnCooldown = false;
    }

    private void Move()
    {
        rb.velocity = Vector3.zero;

        if (canMove)
            behaviour.Move(this, player);
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

    public override void OnStatsChanged()
    {
        
    }
}
