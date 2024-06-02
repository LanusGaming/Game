using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadSaveMenu : SaveMenu
{
    protected override void UpdateSlots(string placeholder = "-", bool interactableWhenMissing = false)
    {
        base.UpdateSlots("-", false);
    }

    public override void SelectSlot(int index)
    {
        if (SaveManager.saves[index] != null)
        {
            SaveManager.SetActive(index);
            controller.StartGame();
        }
    }
}
