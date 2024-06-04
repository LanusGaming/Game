using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class AudioMenu : Menu, ISettingsMenu
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider uiVolumeSlider;
    public Selector speakerModeSelector;

    private bool initialized;

    protected override void Start()
    {
        base.Start();

        Initialize();
        Revert();

        initialized = true;
    }

    private void Initialize()
    {
        masterVolumeSlider.Initialize(0, Settings.Audio.MAX_VOLUME, true);
        musicVolumeSlider.Initialize(0, Settings.Audio.MAX_VOLUME, true);
        sfxVolumeSlider.Initialize(0, Settings.Audio.MAX_VOLUME, true);
        uiVolumeSlider.Initialize(0, Settings.Audio.MAX_VOLUME, true);
        speakerModeSelector.Initialize(new AudioSpeakerMode[] { AudioSpeakerMode.Mono, AudioSpeakerMode.Stereo, AudioSpeakerMode.Quad, AudioSpeakerMode.Surround }, SpeakerModeToString);
    }

    public void Apply(Settings changes)
    {
        if (!initialized)
            return;

        changes.audio.master = (int)masterVolumeSlider.GetValue();
        changes.audio.music = (int)musicVolumeSlider.GetValue();
        changes.audio.sfx = (int)sfxVolumeSlider.GetValue();
        changes.audio.ui = (int)uiVolumeSlider.GetValue();
        changes.audio.mode = (AudioSpeakerMode) speakerModeSelector.GetValue();
    }

    public void Revert()
    {
        masterVolumeSlider.SetValue(Settings.Audio.MasterVolume);
        musicVolumeSlider.SetValue(Settings.Audio.MusicVolume);
        sfxVolumeSlider.SetValue(Settings.Audio.SFXVolume);
        uiVolumeSlider.SetValue(Settings.Audio.UIVolume);
        speakerModeSelector.SetValue(Settings.Audio.SpeakerMode);
    }

    private string SpeakerModeToString(object mode) { return SpeakerModeToString((AudioSpeakerMode) mode); }
    private string SpeakerModeToString(AudioSpeakerMode mode)
    {
        return Enum.GetName(typeof(AudioSpeakerMode), mode);
    }
}
