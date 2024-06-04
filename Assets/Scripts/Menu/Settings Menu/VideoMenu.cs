using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class VideoMenu : Menu, ISettingsMenu
{
    public Selector resolutionSelector;
    public Selector fullScreenModeSelector;
    //public Selector displaySelector;
    public Toggle vsyncToggle;

    private bool initialized;

    protected override void Start()
    {
        base.Start();

        Inititalize();
        Revert();

        initialized = true;
    }

    private void Inititalize()
    {
        resolutionSelector.Initialize(GetResolutions(), ResolutionToString);
        fullScreenModeSelector.Initialize(new FullScreenMode[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed }, FullScreenModeToString);

        //displaySelector.Initialize(GetDisplays());
    }

    public void Apply(Settings changes)
    {
        if (!initialized)
            return;

        changes.video.resolution = (Vector2Int)resolutionSelector.GetValue();
        changes.video.mode = (FullScreenMode)fullScreenModeSelector.GetValue();
        //changes.video.display = (int)displaySelector.GetValue();
        changes.video.vsync = vsyncToggle.GetValue();
    }

    public void Revert()
    {
        resolutionSelector.SetValue(Settings.Video.Resolution);
        fullScreenModeSelector.SetValue(Settings.Video.FullScreenMode);
        //displaySelector.SetValue(Settings.Video.Display);
        vsyncToggle.SetValue(Settings.Video.VSync);
    }

    private Vector2Int[] GetResolutions()
    {
        List<Vector2Int> resolutions = new List<Vector2Int>();

        foreach (var resolution in Screen.resolutions)
        {
            Vector2Int size = new Vector2Int(resolution.width, resolution.height);

            if (resolutions.Contains(size))
                continue;

            resolutions.Add(size);
        }

        return resolutions.ToArray();
    }

    private string ResolutionToString(object resolution) { return ResolutionToString((Vector2Int) resolution); }
    private string ResolutionToString(Vector2Int resolution)
    {
        int gcd = HelperFunctions.GetGreatestCommonDivisor(resolution.x, resolution.y);
        return $"{resolution.x} x {resolution.y} ({resolution.x / gcd}:{resolution.y / gcd})";
    }

    private string FullScreenModeToString(object mode) { return FullScreenModeToString((FullScreenMode) mode); }
    private string FullScreenModeToString(FullScreenMode mode)
    {
        switch (mode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                return "Fullscreen";

            case FullScreenMode.FullScreenWindow:
                return "Borderless";

            case FullScreenMode.Windowed:
                return "Windowed";

            default:
                return "Fullscreen";
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
