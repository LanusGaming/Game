using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Menu : MonoBehaviour
{
    public MenuButton menuButton;

    protected MenuController controller;

    protected virtual void Start()
    {
        controller = FindObjectOfType<MenuController>();
    }

    protected virtual void Update()
    {
        if (InputManager.ExitPressed)
            controller.HideActiveMenu();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        menuButton.Select();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        menuButton.Deselect();
    }
}