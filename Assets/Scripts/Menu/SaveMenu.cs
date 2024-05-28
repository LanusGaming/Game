using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SaveMenu : MonoBehaviour
{
    public GameObject slotObject;
    public Transform slotParent;

    public Button test;

    protected MenuController controller;
    protected TextMeshProUGUI[] slots;

    private void Start()
    {
        controller = FindObjectOfType<MenuController>();

        slots = new TextMeshProUGUI[SaveManager.SAVE_COUNT];

        for (int i = 0; i < SaveManager.SAVE_COUNT; i++)
        {
            RectTransform slot = Instantiate(slotObject, slotParent).GetComponent<RectTransform>();
            slot.localPosition = new Vector3(0, -i * 200);

            slots[i] = slot.GetComponentInChildren<TextMeshProUGUI>();

            SaveSlot saveSlot = slot.gameObject.AddComponent<SaveSlot>();
            saveSlot.index = i;
            saveSlot.menu = this;

            slot.GetComponent<Button>().onClick.AddListener(saveSlot.OnSlotSelected);
        }

        UpdateSlots();
    }

    protected virtual void UpdateSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (SaveManager.saves[i] != null)
                slots[i].text = SaveManager.saves[i].name;
            else
                slots[i].text = "+";
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (slots == null)
            return;

        UpdateSlots();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public abstract void OnSlotSelected(int index);
}

public class SaveSlot : MonoBehaviour
{
    public int index;
    public SaveMenu menu;

    public void OnSlotSelected()
    {
        menu.OnSlotSelected(index);
    }
}