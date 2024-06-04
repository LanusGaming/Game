using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected abstract Stats Stats { get; }
    protected abstract Dictionary<string, StatusEffect> StatusEffects { get; }

    public int Health { get => Stats.Health; protected set => Stats.Health = value; }
    public int MaxHealth { get => Stats.MaxHealth; }
    public float Damage { get => Stats.Damage; }
    public float Defense { get => Stats.Defense; }
    public float MoveSpeed { get => Stats.MoveSpeed; }
    
    public bool invincible;
    public float invinciblityDuration = 0.1f;
    public SpriteRenderer spriteRenderer;
    public Material flashWhiteMaterial;

    private Material originalMaterial;

    public virtual void TakeDamage(float damage)
    {
        if (invincible)
            return;

        Health -= GetDamageTaken(damage);
        Debug.Log($"Entity {gameObject.name} is now at {Health}hp");
        StartCoroutine(FlashOnDamage());

        if (Health == 0)
        {
            invincible = true;
            StartCoroutine(Die());
        }
        else
            StartCoroutine(StartInvincibility());
    }

    public virtual void ApplyStatusEffect(StatusEffect effect)
    {
        if (!StatusEffects.ContainsKey(effect.Name))
        {
            StatusEffects.Add(effect.Name, effect);
            effect.Apply(Stats, this);
        }
        else
            effect.Apply();
    }

    public virtual void RemoveStatusEffect(StatusEffect effect)
    {
        if (!StatusEffects.ContainsKey(effect.Name))
            return;

        StatusEffects[effect.Name].Remove();

        if (StatusEffects[effect.Name].StackCount == 0)
            StatusEffects.Remove(effect.Name);
    }

    protected abstract IEnumerator Die();
    public abstract void OnStatsChanged();

    private int GetDamageTaken(float incomingDamage)
    {
        return (int)Mathf.Max(incomingDamage - Defense, 1f);
    }

    private IEnumerator StartInvincibility()
    {
        invincible = true;
        yield return new WaitForSeconds(invinciblityDuration);
        invincible = false;
    }

    private IEnumerator FlashOnDamage()
    {
        if (originalMaterial is null)
            originalMaterial = spriteRenderer.material;

        spriteRenderer.material = flashWhiteMaterial;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material = originalMaterial;
    }
}