using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoMenu : Menu, ISettingsMenu
{
    public Selector resolutionSelector;
    public Selector fullScreenModeSelector;
    //public Selector displaySelector;
    public Toggle vsyncToggle;

    protected override void Start()
    {
        base.Start();

        Revert();
    }

    public void Apply(Settings changes)
    {
        Resolution resolution = Screen.resolutions[(int)resolutionSelector.GetValue()];
        changes.video.resolution = new Vector2Int(resolution.width, resolution.height);

        switch ((int)fullScreenModeSelector.GetValue())
        {
            case 0:
                changes.video.mode = FullScreenMode.ExclusiveFullScreen;
                break;

            case 1:
                changes.video.mode = FullScreenMode.FullScreenWindow;
                break;

            case 2:
                changes.video.mode = FullScreenMode.Windowed;
                break;
        }

        //changes.video.display = (int)displaySelector.GetValue();

        changes.video.vsync = (bool)vsyncToggle.GetValue();
    }

    public void Revert()
    {
        resolutionSelector.Initialize(GetResolutions());
        resolutionSelector.SetValue(GetResolutionIndex(Settings.Video.Resolution));

        fullScreenModeSelector.SetValue(GetFullScreenModeIndex(Settings.Video.FullScreenMode));

        //displaySelector.Initialize(GetDisplays());
        //displaySelector.SetValue(Settings.Video.Display);

        vsyncToggle.SetValue(Settings.Video.VSync);
    }

    private string[] GetResolutions()
    {
        List<string> resolutions = new List<string>();

        foreach (var resolution in Screen.resolutions)
        {
            int gcd = HelperFunctions.GetGreatestCommonDivisor(resolution.width, resolution.height);
            resolutions.Add($"{resolution.width} x {resolution.height} ({resolution.width / gcd}:{resolution.height / gcd})");
        }

        return resolutions.ToArray();
    }

    private int GetResolutionIndex(Vector2Int resolution)
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (resolution.x == Screen.resolutions[i].width && resolution.y == Screen.resolutions[i].height)
                return i;
        }

        return 0;
    }

    private int GetFullScreenModeIndex(FullScreenMode mode)
    {
        switch (mode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                return 0;

            case FullScreenMode.FullScreenWindow:
                return 1;

            case FullScreenMode.Windowed:
                return 2;

            case FullScreenMode.MaximizedWindow:
                return 2;

            default:
                return 0;
        }
    }

    /*
    private string[] GetDisplays()
    {
        string[] displays = new string[Display.displays.Length];

        for (int i = 0; i < displays.Length; i++)
        {
            displays[i] = $"{i + 1}";
        }

        return displays;
    }
    */
}
