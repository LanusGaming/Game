using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SaveMenu : Menu
{
    public Animator buttonAnimator;
    public GameObject slotObject;
    public Transform slotParent;

    protected SaveSlot[] slots;

    protected override void Start()
    {
        base.Start();

        slots = new SaveSlot[SaveManager.SAVE_COUNT];

        for (int i = 0; i < SaveManager.SAVE_COUNT; i++)
        {
            RectTransform slot = Instantiate(slotObject, slotParent).GetComponent<RectTransform>();
            slot.localPosition = new Vector3(0, -i * 200);

            slots[i] = slot.GetComponent<SaveSlot>();
            slots[i].index = i;
            slots[i].menu = this;
        }

        UpdateSlots();
    }

    protected virtual void UpdateSlots(string placeholder = "+", bool interactableWhenMissing = true)
    {
        foreach (var slot in slots)
        {
            slot.UpdateSlot(placeholder, interactableWhenMissing);
        }
    }

    public override void Show()
    {
        base.Show();

        if (slots == null)
            return;

        UpdateSlots();
    }

    public abstract void SelectSlot(int index);

    public virtual void ClearSlot(int index)
    {
        if (SaveManager.activeSave == SaveManager.saves[index])
        {
            SaveManager.activeSave = null;
            controller.continueButton.SetActive(false);
        }

        SaveManager.saves[index] = null;
        UpdateSlots();
    }
}