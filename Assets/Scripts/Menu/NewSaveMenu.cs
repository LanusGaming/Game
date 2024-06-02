using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewSaveMenu : SaveMenu
{
    public override void SelectSlot(int index)
    {
        Debug.Log(SaveManager.saves[index]);

        if (SaveManager.saves[index] == null)
        {
            SaveManager.CreateNewSave(index);
            controller.StartGame();
        }
    }
}
