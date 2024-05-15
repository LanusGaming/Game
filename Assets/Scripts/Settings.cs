using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy, Normal, Hard, Demonic
}

public static class Settings
{
    public static int seed = 15;
    public static Difficulty Difficulty = Difficulty.Normal;
}
