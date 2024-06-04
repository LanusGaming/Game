using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class ControlsMenu : Menu, ISettingsMenu
{
    public GameObject controlObject;
    public RectTransform controlsParent;
    public GameObject keySelectorOverlay;

    private bool initialized;
    private Settings.Controls controls;
    private Control[] controlObjects;

    protected override void Start()
    {
        base.Start();

        Initialize();
        Revert();

        initialized = true;
    }

    private void Initialize()
    {
        int controlCount = Settings.Controls.AllControls.Length;
        controlObjects = new Control[controlCount];

        float offset = controlObject.GetComponent<RectTransform>().sizeDelta.y;
        controlsParent.sizeDelta = new Vector2(controlsParent.sizeDelta.x, offset * controlCount);

        for (int i = 0; i < controlCount; i++)
        {
            Control control = Instantiate(controlObject, controlsParent).GetComponent<Control>();
            control.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -offset * i);
            control.keySelectorOverlay = keySelectorOverlay;
            controlObjects[i] = control;
        }
    }

    public void Apply(Settings changes)
    {
        if (!initialized)
            return;

        changes.controls = new Settings.Controls(controls);
    }
    public void Revert()
    {
        controls = new Settings.Controls(Settings.Current.controls);
        Settings.Controls.Control[] allControlls = controls.GetAllControls();

        for (int i = 0; i < allControlls.Length; i++)
        {
            controlObjects[i].SetValue(allControlls[i]);
        }
    }
}
