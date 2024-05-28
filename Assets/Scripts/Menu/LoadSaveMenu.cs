using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadSaveMenu : SaveMenu
{
    public override void OnSlotSelected(int index)
    {
        if (SaveManager.saves[index] != null)
        {
            SaveManager.SetActive(index);
            controller.StartGame();
        }
    }
}
