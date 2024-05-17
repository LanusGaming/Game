using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Buff
{
    public float maxHealthMultiplier = 1f;
    public float maxHealthIncrease = 0f;

    public float damageMultiplier = 1f;
    public float damageIncrease = 0f;

    public float moveSpeedMultiplier = 1f;
    public float moveSpeedIncrease = 0f;

    public float defenseMultiplier = 1f;
    public float defenseIncrease = 0f;
}

[Serializable]
public class Stats
{
    private int baseHealth;
    [SerializeField]
    private int baseMaxHealth;
    [SerializeField]
    private float baseDamage;
    [SerializeField]
    private float baseMoveSpeed;
    [SerializeField]
    private float baseDefense;

    [HideInInspector]
    public List<Buff> buffs = new List<Buff>();

    public int health
    {
        get { return baseHealth; }
        set { baseHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    public int maxHealth
    {
        get
        {
            float stat = baseMaxHealth;

            foreach (Buff buff in buffs)
                stat += buff.maxHealthIncrease;
            foreach (Buff buff in buffs)
                stat *= buff.maxHealthMultiplier;

            return (int)stat;
        }
        set { baseMaxHealth = value; }
    }

    public float damage
    {
        get
        {
            float stat = baseDamage;

            foreach (Buff buff in buffs)
                stat += buff.damageIncrease;
            foreach (Buff buff in buffs)
                stat *= buff.damageMultiplier;

            return (int)stat;
        }
        set { baseDamage = value; }
    }

    public float moveSpeed
    {
        get
        {
            float stat = baseMoveSpeed;

            foreach (Buff buff in buffs)
                stat += buff.moveSpeedIncrease;
            foreach (Buff buff in buffs)
                stat *= buff.moveSpeedMultiplier;

            return stat;
        }
        set { baseMoveSpeed = value; }
    }

    public float defense
    {
        get
        {
            float stat = baseDefense;

            foreach (Buff buff in buffs)
                stat += buff.defenseIncrease;
            foreach (Buff buff in buffs)
                stat *= buff.defenseMultiplier;

            return stat;
        }
        set { baseDefense = value; }
    }
}

public abstract class Entity : MonoBehaviour
{
    public abstract int health { get; set; }
    public abstract int maxHealth { get; set; }
    public abstract float damage { get; set; }
    public abstract float moveSpeed { get; set; }
    public abstract float defense { get; set; }

    public SpriteRenderer spriteRenderer;
    public Sprite sprite;

    [SerializeField]
    protected Stats stats = new Stats();

    private Sprite whiteFlash;

    public static int CalculateDamage(float incomingDamage, Entity entity)
    {
        return (int)Mathf.Max(incomingDamage - entity.defense, 1f);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= CalculateDamage(damage, this);
        StartCoroutine(FlashOnDamage());

        if (health == 0)
            Die();
    }

    public virtual void ApplyBuff(Buff buff, float duration)
    {
        stats.buffs.Add(buff);

        if (duration > 0f)
            StartCoroutine(BuffCountdown(buff, duration));
    }

    public virtual void RemoveBuff(Buff buff)
    {
        stats.buffs.Remove(buff);
    }

    public virtual void ClearBuffs()
    {
        stats.buffs.Clear();
    }

    protected abstract void Die();

    private IEnumerator FlashOnDamage()
    {
        if (whiteFlash is null)
            whiteFlash = HelperFunctions.GenerateWhiteFlashSprite(spriteRenderer.sprite);

        spriteRenderer.sprite = whiteFlash;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.sprite = sprite;
    }

    private IEnumerator BuffCountdown(Buff buff, float duration)
    {
        yield return new WaitForSeconds(duration);
        RemoveBuff(buff);
    }
}