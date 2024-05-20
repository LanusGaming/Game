using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    public static int health
    {
        get { return stats.health; }
        set { stats.health = value; }
    }
    public static int maxHealth
    {
        get { return stats.maxHealth; }
        set { stats.maxHealth = value; }
    }
    public static float damage
    {
        get { return stats.damage; }
        set { stats.damage = value; }
    }
    public static float moveSpeed
    {
        get { return stats.moveSpeed; }
        set { stats.moveSpeed = value; }
    }
    public static float defense
    {
        get { return stats.defense; }
        set { stats.defense = value; }
    }

    public static Stats stats = new Stats();
}