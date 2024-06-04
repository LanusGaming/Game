using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game
{
    [Serializable]
    public enum Difficulty
    {
        Easy, Normal, Hard, Demonic
    }

    [Serializable]
    public class SaveData
    {
        [Serializable]
        public class Config
        {
            public static int Seed { get => Current.config.seed; }
            public static Difficulty Difficulty { get => Current.config.difficulty; }

            public static float DamageMultiplier { get => Current.config.GetDamageMultiplier(); }
            public static float HealthMultiplier { get => Current.config.GetHealthMultiplier(); }

            public int seed = 0;
            public Difficulty difficulty = Difficulty.Normal;

            public Config() { }

            public Config(Config other)
            {
                seed = other.seed;
                difficulty = other.difficulty;
            }

            public float GetDamageMultiplier()
            {
                switch (difficulty)
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

            public float GetHealthMultiplier()
            {
                switch (difficulty)
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

        [Serializable]
        public class Upgrades
        {
            public Upgrades() { }

            public Upgrades(Upgrades other) { }
        }

        public static SaveData Current { get; private set; }

        public Config config = new Config();
        public Upgrades upgrades = new Upgrades();
        public RunData activeRun = null;
        public PlayerData basePlayer = new PlayerData();

        public SaveData() { }

        public SaveData(SaveData other)
        {
            config = new Config(other.config);
            upgrades = new Upgrades(other.upgrades);
        }

        public SaveData(Config config, Upgrades upgrades)
        {
            this.config = new Config(config);
            this.upgrades = new Upgrades(upgrades);
        }

        public static void Set(SaveData saveData)
        {
            Current = new SaveData(saveData);
        }

        public static void StartRun()
        {
            StartRun(new RunData());
        }

        public static void StartRun(RunData run)
        {
            Current.activeRun = run;
        }
    }
}