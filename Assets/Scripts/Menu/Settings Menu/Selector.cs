using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MenuItem
{
    public string[] values;
    public TextMeshProUGUI valueTextMesh;
    public Button moveLeftButton;
    public Button moveRightButton;
    
    private int value;

    public void Initialize(string[] values)
    {
        this.values = values;
    }

    public void ChangeValue(int delta)
    {
        SetValue(value + delta);
    }

    public override object GetValue()
    {
        return value;
    }

    public override void SetValue(object value)
    {
        this.value = Mathf.Clamp((int) value, 0, values.Length - 1);
        valueTextMesh.text = values[this.value];

        if (this.value == 0)
            moveLeftButton.interactable = false;
        else
            moveLeftButton.interactable = true;

        if (this.value == values.Length - 1)
            moveRightButton.interactable = false;
        else
            moveRightButton.interactable = true;

        base.SetValue(value);
    }
}
