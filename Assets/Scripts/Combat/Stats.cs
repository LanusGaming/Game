using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    public int Health { get => health; set => health = Mathf.Clamp(value, 0, MaxHealth); }
    public int MaxHealth { get => (int)(baseMaxHealth * maxHealthMultiplier); }
    public int Damage { get => (int)(baseDamage * damageMultiplier); }
    public int Defense { get => (int)(baseDefense * defenseMultiplier); }
    public int MoveSpeed { get => (int)(baseMoveSpeed * moveSpeedMultiplier); }

    private int health;

    public int baseMaxHealth = 100;
    public float maxHealthMultiplier = 1f;

    public float baseDamage = 1f;
    public float damageMultiplier = 1f;

    public float baseDefense = 0f;
    public float defenseMultiplier = 1f;

    public float baseMoveSpeed = 5f;
    public float moveSpeedMultiplier = 1f;

    public Stats() { health = MaxHealth; }

    public Stats(Stats other)
    {
        baseMaxHealth = other.baseMaxHealth;
        maxHealthMultiplier = other.maxHealthMultiplier;
        baseDamage = other.baseDamage;
        damageMultiplier = other.damageMultiplier;
        baseDefense = other.baseDefense;
        defenseMultiplier = other.defenseMultiplier;
        baseMoveSpeed = other.baseMoveSpeed;
        moveSpeedMultiplier = other.moveSpeedMultiplier;
    }

    public Stats(int baseMaxHealth, float baseDamage, float baseDefense, float baseMoveSpeed, float maxHealthMultiplier = 1f, float damageMultiplier = 1f, float defenseMultiplier = 1f, float moveSpeedMultiplier = 1f)
    {
        this.baseMaxHealth = baseMaxHealth;
        this.maxHealthMultiplier = maxHealthMultiplier;
        this.baseDamage = baseDamage;
        this.damageMultiplier = damageMultiplier;
        this.baseDefense = baseDefense;
        this.defenseMultiplier = defenseMultiplier;
        this.baseMoveSpeed = baseMoveSpeed;
        this.moveSpeedMultiplier = moveSpeedMultiplier;

        health = MaxHealth;
    }
}