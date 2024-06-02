using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public GameObject clearButton;

    [HideInInspector]
    public int index;
    [HideInInspector]
    public SaveMenu menu;

    public void UpdateSlot(string placeholder, bool interactableWhenMissing)
    {
        Save save = SaveManager.saves[index];

        textMesh.text = (save == null) ? placeholder : save.name;
        clearButton.SetActive(save != null);
        GetComponent<Button>().interactable = (save == null) ? interactableWhenMissing : !interactableWhenMissing;
    }

    public void SelectSlot()
    {
        menu.SelectSlot(index);
    }

    public void ClearSlot()
    {
        menu.ClearSlot(index);
    }
}
