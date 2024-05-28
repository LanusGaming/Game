using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewSaveMenu : SaveMenu
{
    public override void OnSlotSelected(int index)
    {
        Debug.Log(index);

        if (SaveManager.saves[index] == null)
        {
            SaveManager.CreateNewSave(index);
            controller.StartGame();
        }
    }
}
