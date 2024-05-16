using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy, Normal, Hard, Demonic
}

public class GameSettings
{
    public int seed = 0;
    public Difficulty Difficulty = Difficulty.Normal;
}

public class ControlSettings
{
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;

    public KeyCode interact = KeyCode.E;
}

public class Upgrades
{

}

public static class Configuration
{
    public static GameSettings game = new GameSettings();
    public static ControlSettings controls = new ControlSettings();
    public static Upgrades upgrades = new Upgrades();
}
