using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.VolumeComponent;

[Serializable]
public class Settings
{
    [Serializable]
    public class Gameplay
    {
        public Gameplay() { }

        public Gameplay(Gameplay other)
        {

        }

        public static bool operator ==(Gameplay a, Gameplay b)
        {
            return true;
        }

        public static bool operator !=(Gameplay a, Gameplay b)
        {
            return !(a == b);
        }
    }

    [Serializable]
    public class Video
    {
        public static Vector2Int Resolution { get => Instance.video.resolution; }
        public static FullScreenMode FullScreenMode { get => Instance.video.mode; }
        //public static int Display { get => Instance.video.display; }
        public static bool VSync { get => Instance.video.vsync; }

        public Vector2Int resolution = new Vector2Int(Screen.width, Screen.height);
        public FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
        //public int display = UnityEngine.Display.displays.ToList().IndexOf(UnityEngine.Display.main);
        public bool vsync = true;

        public Video() { }

        public Video(Video other)
        {
            resolution = other.resolution;
            mode = other.mode;
            //display = other.display;
            vsync = other.vsync;
        }

        public static bool operator ==(Video a, Video b)
        {
            return a.resolution == b.resolution && a.mode == b.mode /*&& a.display == b.display*/ && a.vsync == b.vsync;
        }

        public static bool operator !=(Video a, Video b)
        {
            return !(a == b);
        }
    }

    [Serializable]
    public class Audio
    {
        public const int MAX_VOLUME = 100;

        public static int MasterVolume { get => Instance.audio.master; }
        public static int MusicVolume { get => Instance.audio.music; }
        public static int SFXVolume { get => Instance.audio.sfx; }
        public static int UIVolume { get => Instance.audio.ui; }
        public static AudioSpeakerMode SpeakerMode { get => Instance.audio.mode; }

        public int master = 50;
        public int music = 100;
        public int sfx = 100;
        public int ui = 100;

        public AudioSpeakerMode mode = AudioSpeakerMode.Stereo;

        public Audio() { }

        public Audio(Audio other)
        {
            master = other.master;
            music = other.music;
            sfx = other.sfx;
            ui = other.ui;
            mode = other.mode;
        }

        public static bool operator ==(Audio a, Audio b)
        {
            return a.master == b.master && a.sfx == b.sfx && a.ui == b.ui && a.music == b.music && a.mode == b.mode;
        }

        public static bool operator !=(Audio a, Audio b)
        {
            return !(a == b);
        }
    }

    [Serializable]
    public class Controls
    {
        [Serializable]
        public class Control
        {
            public string name;
            public KeyCode primaryKey;
            public KeyCode secondaryKey;

            public Control(string name, KeyCode primaryKey = KeyCode.None, KeyCode secondaryKey = KeyCode.None)
            {
                this.name = name;
                this.primaryKey = primaryKey;
                this.secondaryKey = secondaryKey;
            }

            public Control(Control other)
            {
                name = other.name;
                primaryKey = other.primaryKey;
                secondaryKey = other.secondaryKey;
            }

            public static bool operator ==(Control a, Control b)
            {
                return a.primaryKey == b.primaryKey && a.secondaryKey == b.secondaryKey;
            }

            public static bool operator !=(Control a, Control b)
            {
                return !(a == b);
            }
        }

        public static bool MoveUpPressed { get => isPressed(Instance.controls.moveUp); }
        public static bool MoveDownPressed { get => isPressed(Instance.controls.moveDown); }
        public static bool MoveLeftPressed { get => isPressed(Instance.controls.moveLeft); }
        public static bool MoveRightPressed { get => isPressed(Instance.controls.moveRight); }
        public static bool InteractPressed { get => isDown(Instance.controls.interact); }
        public static bool OpenMapPressed { get => isDown(Instance.controls.openMap); }
        public static bool ExitPressed { get => isDown(Instance.controls.exit); }

        public Control moveUp = new Control("Up", KeyCode.W);
        public Control moveDown = new Control("Down", KeyCode.S);
        public Control moveLeft = new Control("Left", KeyCode.A);
        public Control moveRight = new Control("Right", KeyCode.D);
        public Control interact = new Control("Interact", KeyCode.E);
        public Control openMap = new Control("Map", KeyCode.M);
        public Control exit = new Control("Exit", KeyCode.Escape);

        public Controls() { }

        public Controls(Controls other)
        {
            Control[] controls = GetAllControls();
            Control[] controlsOther = other.GetAllControls();

            for (int i = 0; i < controls.Length; i++)
                controls[i] = new Control(controlsOther[i]);
        }

        public static bool operator ==(Controls a, Controls b)
        {
            Control[] controlsA = a.GetAllControls();
            Control[] controlsB = b.GetAllControls();

            for (int i = 0; i < controlsA.Length; i++)
                if (controlsA[i] != controlsB[i])
                    return false;

            return true;
        }

        public static bool operator !=(Controls a, Controls b)
        {
            return !(a == b);
        }

        public Control[] GetAllControls()
        {
            List<Control> controls = new List<Control>();

            foreach (var member in typeof(Controls).GetMembers())
                if (member.MemberType == MemberTypes.Field)
                    controls.Add(((FieldInfo)member).GetValue(this) as Control);

            return controls.ToArray();
        }

        private static bool isPressed(Control control)
        {
            return Input.GetKey(control.primaryKey) || Input.GetKey(control.secondaryKey);
        }

        private static bool isDown(Control control)
        {
            return Input.GetKeyDown(control.primaryKey) || Input.GetKeyDown(control.secondaryKey);
        }
    }

    public static class SettingsManager
    {
        private const string SETTINGS_FILE_NAME = "settings.json";

        public static void SaveSettings()
        {
            string path = $"{Application.persistentDataPath}/{SETTINGS_FILE_NAME}";

            File.WriteAllText(path, JsonUtility.ToJson(Settings.Instance, true));
        }

        public static void LoadSettings()
        {
            string path = $"{Application.persistentDataPath}/{SETTINGS_FILE_NAME}";

            if (!File.Exists(path))
            {
                Instance = new Settings();
                SaveSettings();
                return;
            }

            Instance = JsonUtility.FromJson(File.ReadAllText(path), typeof(Settings)) as Settings;
        }
    }

    public static Settings Instance { get; private set; }
    public static AudioMixer Mixer { private get;  set; }

    public Gameplay gameplay;
    public Video video;
    public Audio audio;
    public Controls controls;

    public Settings()
    {
        gameplay = new Gameplay();
        video = new Video();
        audio = new Audio();
        controls = new Controls();
    }

    public Settings(Settings other)
    {
        gameplay = new Gameplay(other.gameplay);
        video = new Video(other.video);
        audio = new Audio(other.audio);
        controls = new Controls(other.controls);
    }

    public Settings(Gameplay gameplay, Video video, Audio audio, Controls controls)
    {
        this.gameplay = gameplay;
        this.video = video;
        this.audio = audio;
        this.controls = controls;
    }

    public static bool operator ==(Settings a, Settings b)
    {
        if (a is null && b is null) return true;

        if (a is null || b is null) return false;

        if (a.gameplay != b.gameplay) return false;
        if (a.video != b.video) return false;
        if (a.audio != b.audio) return false;
        if (a.controls != b.controls) return false;

        return true;
    }

    public static bool operator !=(Settings a, Settings b)
    {
        return !(a == b);
    }

    public static void Apply()
    {
        // Video
        //Camera.main.targetDisplay = Video.Display;
        Screen.SetResolution(Video.Resolution.x, Video.Resolution.y, Video.FullScreenMode);
        QualitySettings.vSyncCount = Video.VSync ? 1 : 0;

        // Audio
        Mixer.SetFloat("masterVolume", -80 + Audio.MasterVolume);
        Mixer.SetFloat("musicVolume", -80 + Audio.MusicVolume);
        Mixer.SetFloat("sfxVolume", -80 + Audio.SFXVolume);
        Mixer.SetFloat("uiVolume", -80 + Audio.UIVolume);

        AudioSettings.speakerMode = Audio.SpeakerMode;
    }

    public static void Apply(Settings settings)
    {
        Instance = new Settings(settings);

        Apply();
    }
}

