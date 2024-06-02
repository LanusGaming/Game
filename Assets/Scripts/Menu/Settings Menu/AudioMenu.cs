using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMenu : Menu, ISettingsMenu
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider uiVolumeSlider;
    public Selector speakerModeSelector;

    protected override void Start()
    {
        base.Start();

        Revert();
    }

    public void Apply(Settings changes)
    {
        changes.audio.master = (int)(float)masterVolumeSlider.GetValue();
        changes.audio.music = (int)(float)musicVolumeSlider.GetValue();
        changes.audio.sfx = (int)(float)sfxVolumeSlider.GetValue();
        changes.audio.ui = (int)(float)uiVolumeSlider.GetValue();

        switch ((int)speakerModeSelector.GetValue())
        {
            case 0:
                changes.audio.mode = AudioSpeakerMode.Mono;
                break;

            case 1:
                changes.audio.mode = AudioSpeakerMode.Stereo;
                break;

            case 2:
                changes.audio.mode = AudioSpeakerMode.Quad;
                break;

            case 3:
                changes.audio.mode = AudioSpeakerMode.Surround;
                break;

            default:
                changes.audio.mode = AudioSpeakerMode.Stereo;
                break;
        }
    }

    public void Revert()
    {
        masterVolumeSlider.Initialize(0, Settings.Audio.MAX_VOLUME, true);
        masterVolumeSlider.SetValue((float)Settings.Audio.MasterVolume);

        musicVolumeSlider.Initialize(0, Settings.Audio.MAX_VOLUME, true);
        musicVolumeSlider.SetValue((float)Settings.Audio.MusicVolume);

        sfxVolumeSlider.Initialize(0, Settings.Audio.MAX_VOLUME, true);
        sfxVolumeSlider.SetValue((float)Settings.Audio.SFXVolume);

        uiVolumeSlider.Initialize(0, Settings.Audio.MAX_VOLUME, true);
        uiVolumeSlider.SetValue((float)Settings.Audio.UIVolume);

        speakerModeSelector.SetValue(GetSpeakerModeIndex(Settings.Audio.SpeakerMode));
    }

    private int GetSpeakerModeIndex(AudioSpeakerMode mode)
    {
        switch (mode)
        {
            case AudioSpeakerMode.Mono:
                return 0;

            case AudioSpeakerMode.Stereo:
                return 1;

            case AudioSpeakerMode.Quad:
                return 2;

            case AudioSpeakerMode.Surround:
                return 3;

            default:
                return 1;
        }
    }
}
