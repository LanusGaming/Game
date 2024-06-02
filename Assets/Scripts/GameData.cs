using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public enum Difficulty
{
    Easy, Normal, Hard, Demonic
}

[Serializable]
public class Config
{
    public int seed = 0;
    public Difficulty Difficulty = Difficulty.Normal;

    public float GetDamageMultiplier()
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

    public float GetHealthMultiplier()
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

[Serializable]
public class Upgrades
{

}

[Serializable]
public class GameData
{
    public static GameData Instance { get { return new GameData(Config, Upgrades); } }
    public static Config Config { get; private set; }
    public static Upgrades Upgrades { get; private set; }

    public Config config;
    
    public Upgrades upgrades;

    public GameData()
    {
        config = new Config();
        upgrades = new Upgrades();
    }

    public GameData(Config config, Upgrades upgrades)
    {
        this.config = config;
        this.upgrades = upgrades;
    }

    public static void Set()
    {
        Config = new Config();
        Upgrades = new Upgrades();
    }

    public static void Set(GameData gameData)
    {
        Config = gameData.config;
        Upgrades = gameData.upgrades;
    }
}

[Serializable]
public class Save
{
    public string name;
    public bool isActive;
    public GameData gameData;

    public Save(string name)
    {
        this.name = name;
        isActive = false;
        gameData = new GameData();
    }

    public Save(string name, bool isActive, GameData gameData)
    {
        this.name = name;
        this.isActive = isActive;
        this.gameData = gameData;
    }
}

public static class SaveManager
{
    public static Save activeSave;
    public static Save[] saves = new Save[SAVE_COUNT];

    public const int SAVE_COUNT = 3;
    public const string SAVE_PREFIX = "save";

    public static void SaveAll()
    {
        for (int i = 0; i < SAVE_COUNT; i++)
        {
            Save save = saves[i];

            if (save == null)
                Clear(i);
            else
                Save(save);
        }
    }

    public static void CreateNewSave(int index)
    {
        Save save = new Save($"{SAVE_PREFIX}_{index}");
        saves[index] = save;
        SetActive(index);
    }

    public static void LoadSaves()
    {
        for (int i = 0; i < SAVE_COUNT; i++)
        {
            saves[i] = Load($"{SAVE_PREFIX}_{i}");
            if (saves[i] != null && saves[i].isActive)
                activeSave = saves[i];
        }
    }

    public static void SetActive(int index)
    {
        if (saves[index] == null)
            return;

        if (activeSave != null)
            activeSave.isActive = false;

        activeSave = saves[index];
        activeSave.isActive = true;
    }

    private static void Save(Save save)
    {
        string path = $"{Application.persistentDataPath}/{save.name}";

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Create);

        binaryFormatter.Serialize(fileStream, save);
        fileStream.Close();
    }

    private static Save Load(string name)
    {
        string path = $"{Application.persistentDataPath}/{name}";

        if (!File.Exists(path))
            return null;

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Open);

        Save save = binaryFormatter.Deserialize(fileStream) as Save;
        fileStream.Close();

        return save;
    }

    private static void Clear(int index)
    {
        string path = $"{Application.persistentDataPath}/{SAVE_PREFIX}_{index}";

        if (File.Exists(path))
            File.Delete(path);
    }
}