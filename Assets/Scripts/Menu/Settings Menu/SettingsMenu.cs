using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenu : Menu
{
    public Menu defaultMenu;
    public GameObject applyButtons;

    private static SettingsMenu instance;

    private Menu activeMenu;
    private Settings changes;
    private bool initialized;

    private void Awake()
    {
        instance = this;

        StartCoroutine(WaitForEndOfStart());
    }

    public static void Updated(ISettingsMenu menu)
    {
        if (!instance.initialized)
            return;

        Settings changes;

        if (instance.changes == null)
            changes = new Settings(Settings.Instance);
        else
            changes = instance.changes;

        menu.Apply(changes);

        if (changes == Settings.Instance)
        {
            instance.changes = null;
            instance.applyButtons.SetActive(false);
        }
        else
        {
            instance.changes = changes;
            instance.applyButtons.SetActive(true);
        }
    }

    public override void Show()
    {
        base.Show();
        ShowMenu(defaultMenu);
    }

    public override void Hide()
    {
        base.Hide();
        HideActiveMenu();
    }

    public void HideActiveMenu()
    {
        if (activeMenu != null)
            activeMenu.Hide();
    }

    public void ShowMenu(Menu menu)
    {
        HideActiveMenu();
        menu.Show();
        activeMenu = menu;
    }

    public void ApplyChanges()
    {
        Settings.Apply(changes);
        changes = null;
        instance.applyButtons.SetActive(false);
        Hide();
    }

    public void RevertChanges()
    {
        ISettingsMenu[] menus = GetComponentsInChildren<ISettingsMenu>();

        foreach (ISettingsMenu menu in menus)
            menu.Revert();

        instance.applyButtons.SetActive(false);
    }

    private IEnumerator WaitForEndOfStart()
    {
        initialized = false;
        yield return new WaitForEndOfFrame();
        initialized = true;
    }
}

public interface ISettingsMenu
{
    public void Apply(Settings changes);

    public void Revert();
}