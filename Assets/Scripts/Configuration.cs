using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    public enum Difficulty
    {
        Easy, Normal, Hard, Demonic
    }

    public static class Game
    {
        public static int seed = 0;
        public static Difficulty Difficulty = Difficulty.Normal;

        public static float GetDamageMultiplier()
        {
            switch (Difficulty)
            {
                case Difficulty.Easy:
                    return 0.75f;

                case Difficulty.Normal:
                    return 1f;

                case Difficulty.Hard:
                    return 1.5f;

                case Difficulty.Demonic:
                    return 2f;
            }

            return 1f;
        }

        public static float GetHealthMultiplier()
        {
            switch (Difficulty)
            {
                case Difficulty.Easy:
                    return 0.75f;

                case Difficulty.Normal:
                    return 1f;

                case Difficulty.Hard:
                    return 1.5f;

                case Difficulty.Demonic:
                    return 2f;
            }

            return 1f;
        }
    }

    public static class Controls
    {
        public static KeyCode moveUp = KeyCode.W;
        public static KeyCode moveDown = KeyCode.S;
        public static KeyCode moveLeft = KeyCode.A;
        public static KeyCode moveRight = KeyCode.D;

        public static KeyCode interact = KeyCode.E;
    }

    public static class Upgrades
    {

    }
}
