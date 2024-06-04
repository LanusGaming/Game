using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class PlayerData
    {
        private static PlayerData Current {
            get
            {
                if (RunData.ActiveRun == null)
                    return SaveData.Current.basePlayer;
                else
                    return RunData.ActiveRun.playerData;
            }
        }

        public static Stats Stats { get => Current.stats; }
        public static Dictionary<string, StatusEffect> StatusEffects { get => Current.statusEffects; }

        public Stats stats = new Stats(100, 10f, 10f, 10f);
        public Dictionary<string, StatusEffect> statusEffects = new Dictionary<string, StatusEffect>();

        public PlayerData() { }

        public PlayerData(PlayerData other)
        {
            stats = new Stats(other.stats);
        }

        public PlayerData(Stats stats)
        {
            this.stats = new Stats(stats);
        }
    }
}