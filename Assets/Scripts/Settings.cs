using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Controls
{
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode interact = KeyCode.E;
    public KeyCode map = KeyCode.M;
    public KeyCode exit = KeyCode.Escape;
}

[Serializable]
public class Settings
{
    public static Settings Instance { get { return new Settings(Controls); } }
    public static Controls Controls { get; private set; }

    public Controls controls;

    public Settings()
    {
        controls = new Controls();
    }

    public Settings(Controls controls)
    {
        this.controls = controls;
    }

    public static void Set()
    {
        Controls = new Controls();
    }

    public static void Set(Settings settings)
    {
        Controls = settings.controls;
    }
}

public static class SettingsManager
{
    private const string SETTINGS_FILE_NAME = "settings";

    public static void SaveSettings()
    {
        string path = $"{Application.persistentDataPath}/{SETTINGS_FILE_NAME}";

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Create);

        binaryFormatter.Serialize(fileStream, Settings.Instance);
        fileStream.Close();
    }

    public static void LoadSettings()
    {
        string path = $"{Application.persistentDataPath}/{SETTINGS_FILE_NAME}";

        if (!File.Exists(path))
        {
            Settings.Set();
            SaveSettings();
            return;
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Open);

        Settings.Set(binaryFormatter.Deserialize(fileStream) as Settings);
        fileStream.Close();
    }
}