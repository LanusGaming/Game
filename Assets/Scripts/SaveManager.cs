using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Game;

[Serializable]
public class Save
{
    public string name;
    public bool isActive;
    public SaveData saveData;

    public Save(string name)
    {
        this.name = name;
        isActive = false;
        saveData = new SaveData();
    }

    public Save(string name, bool isActive, SaveData saveData)
    {
        this.name = name;
        this.isActive = isActive;
        this.saveData = saveData;
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

    public static void Apply()
    {
        SaveData.Set(activeSave.saveData);
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
