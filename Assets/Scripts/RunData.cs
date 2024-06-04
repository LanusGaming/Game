using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RunData
{
    public static RunData ActiveRun { get => SaveData.Current.activeRun; }

    public PlayerData playerData = new PlayerData(SaveData.Current.basePlayer);
}
