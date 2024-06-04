using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Game;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Entity
{
    public static Player Instance { get; private set; }

    protected override Stats Stats { get => PlayerData.Stats; }
    protected override Dictionary<string, StatusEffect> StatusEffects { get => PlayerData.StatusEffects; }

    [HideInInspector]
    public bool active = false;

    private Rigidbody2D rb;

    void Awake() { Instance = this; }

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
            velocity = InputManager.MoveDirection;

            if (InputManager.MoveRight && !InputManager.MoveLeft)
                spriteRenderer.flipX = false;
            else if (InputManager.MoveLeft)
                spriteRenderer.flipX = true;
        }

        rb.velocity = velocity * MoveSpeed;
    }

    protected override IEnumerator Die()
    {
        Debug.Log("Player has died!");
        yield return new WaitForSeconds(invinciblityDuration);
        invincible = false;
    }

    public override void OnStatsChanged()
    {

    }
}
